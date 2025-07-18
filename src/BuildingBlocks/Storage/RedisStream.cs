using codecrafters_redis.BuildingBlocks.Exceptions;

namespace codecrafters_redis.BuildingBlocks.Storage;

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

    // public StreamEntry[] Range(string start, string end, int? count = null)
    // {
    //     lock (_syncLock)
    //     {
    //         var startId = ParseId(start);
    //         var endId = ParseId(end);
    //         
    //         return _entries
    //             .Where(kv => 
    //                 CompareIds(kv.Key, startId) >= 0 && 
    //                 CompareIds(kv.Key, endId) <= 0)
    //             .Take(count ?? int.MaxValue)
    //             .Select(kv => kv.Value)
    //             .ToArray();
    //     }
    // }

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