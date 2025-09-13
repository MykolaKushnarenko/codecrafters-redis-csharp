using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.Exceptions;

namespace DotRedis.BuildingBlocks.Storage;

public class RedisStream
{
    private readonly SortedDictionary<string, StreamEntry> _entries = new();
    private readonly Lock _syncLock = new();

    public string AddEntry(string id, Dictionary<string, RedisValue> fields)
    {
        lock (_syncLock)
        {
            Validate(id);

            if (id.Contains("*"))
            {
                id = GenerateAutoId(id);
            }
            
            var entry = new StreamEntry(id, fields);
            _entries.Add(id, entry);
            return id;
        }
    }

    public StreamEntry[] Range(string start, string end, int? count = null)
    {
        if (start == "-")
        {
            start = _entries.Keys.First();
        }

        if (end == "+")
        {
            end = _entries.Keys.Last();
        }
        
        lock (_syncLock)
        {
            return _entries
                .Where(kv => 
                    CompareIds(kv.Key, start) >= 0 && 
                    CompareIds(kv.Key, end) <= 0)
                .Take(count ?? int.MaxValue)
                .Select(kv => kv.Value)
                .ToArray();
        }
    }
    
    public StreamEntry[] Read(string id)
    {
        lock (_syncLock)
        {
            return _entries
                .Where(kv => 
                    CompareIds(kv.Key, id) > 0 )
                .Select(kv => kv.Value)
                .ToArray();
        }
    }
    
    public StreamEntry ReadLatest()
    {
        lock (_syncLock)
        {
            return _entries
                .Last()
                .Value;
        }
    }
    
    private int CompareIds(string id1, string id2)
    {
        if (id1 == "-") return -1;
        if (id1 == "+") return 1;
        if (id2 == "-") return 1;
        if (id2 == "+") return -1;

        var parts1 = id1.Split('-');
        var parts2 = id2.Split('-');

        // Compare milliseconds
        var ms1 = long.Parse(parts1[0]);
        var ms2 = long.Parse(parts2[0]);
        if (ms1 != ms2) return ms1.CompareTo(ms2);

        // Compare sequence numbers if milliseconds equal
        var seq1 = long.Parse(parts1[1]);
        var seq2 = long.Parse(parts2[1]);
        return seq1.CompareTo(seq2);
    }

    private void Validate(string id)
    {
        var newStreamTimeAndSequence = id.Split('-');

        if (newStreamTimeAndSequence[0] == "*" || newStreamTimeAndSequence[1] == "*")
        {
            return;
        }
        
        var newStreamTimestamp = long.Parse(newStreamTimeAndSequence[0]);
        var newStreamSequence = long.Parse(newStreamTimeAndSequence[1]);

        if (newStreamSequence == 0 && newStreamTimestamp == 0)
        {
            throw new RedisException("The ID specified in XADD must be greater than 0-0");
        }
        
        if (_entries.Keys.Count == 0)
        {
            return;
        }

        var lastStreamTimeAndSequence = _entries.Keys.Last().Split('-');
        var lastStreamTimestamp = long.Parse(lastStreamTimeAndSequence[0]);
        var lastStreamSequence = long.Parse(lastStreamTimeAndSequence[1]);

        if (lastStreamTimestamp < newStreamTimestamp)
        {
            return;
        }

        if (lastStreamTimestamp == newStreamTimestamp &&
            lastStreamSequence < newStreamSequence)
        {
            return;
        }
        
        throw new RedisException($"The ID specified in XADD is equal or smaller than the target stream top item");
    }

    private string GenerateAutoId(string id)
    {
        long timestamp = 0;
        long sequence = 0;
        var idsTimestampAndSequence = id.Split('-');

        if (idsTimestampAndSequence.Length != 2 && idsTimestampAndSequence[0] == "*")
        {
            return $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{sequence}";
        }
        
        timestamp = idsTimestampAndSequence[0] == "*" ? 0 : long.Parse(idsTimestampAndSequence[0]);
        
        switch (idsTimestampAndSequence[1])
        {
            case "*" when _entries.Count != 0 && _entries.Last().Key.StartsWith(timestamp.ToString()):
            {
                var lastSeq = _entries.Last().Key.Split('-')[1];
                sequence = int.Parse(lastSeq) + 1;
                break;
            }
            case "*":
                sequence = timestamp == 0 ? 1 : 0;
                break;
            default:
                sequence = long.Parse(idsTimestampAndSequence[1]);
                break;
        }
        
        return $"{timestamp}-{sequence}";
    }
}