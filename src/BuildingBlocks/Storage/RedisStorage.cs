using System.Collections.Concurrent;
using codecrafters_redis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Storage;

public class RedisStorage
{
    private readonly ConcurrentDictionary<string, RedisValue> _data;
    private readonly ConcurrentDictionary<string, RedisStream> _streams;
    
    public RedisStorage()
    {
        _data = new ConcurrentDictionary<string, RedisValue>();
        _streams = new ConcurrentDictionary<string, RedisStream>();
    }
    
    public RedisValue Get(string key)
    {
        if (_data.TryGetValue(key, out var value))
        {
            return value;
        }
        return RedisValue.Null;
    }
    
    public void Set(string key, RedisValue value)
    {
        if (value.Type == RedisValueType.Stream)
        {
            throw new InvalidOperationException("Use XADD for stream operations");
        }
        _data[key] = value;
        // Handle expiry...
    }
    
    public string[] GetAllKeys()
    {
       return _data.Keys.ToArray();
        // Handle expiry...
    }

    public void Remove(string key)
    {
        _data.TryRemove(key, out _);
    }
    
    public string XAdd(string key, string id, Dictionary<string, RedisValue> fields)
    {
        var stream = _streams.GetOrAdd(key, _ => new RedisStream());
        return stream.AddEntry(id, fields);
    }

    public RedisStream GetStream(string key)
    {
        return _streams.GetValueOrDefault(key, null);
    }

    public int RPush(string key, RedisValue value)
    {
        var redisValue = _data.GetOrAdd(key, _ => RedisValue.Create(new List<RedisValue>()));
        
        var list = (List<RedisValue>)redisValue.Value;
        list.Add(value);

        _data[key] = redisValue;
        
        return list.Count;
    }
    
    public int LPush(string key, RedisValue value)
    {
        var redisValue = _data.GetOrAdd(key, _ => RedisValue.Create(new List<RedisValue>()));
        
        var list = (List<RedisValue>)redisValue.Value;
        list.Insert(0, value);

        _data[key] = redisValue;
        
        return list.Count;
    }

    public int ZAdd(string key, double score, string value)
    {
        var redisValue = _data.GetOrAdd(key, _ => RedisValue.Create(new SortedDictionary<double, string>()));

        var sortedSet = (SortedDictionary<double, string>)redisValue.Value;

        var itemsCount = sortedSet.Count;
        
        sortedSet[score] = value;

        return itemsCount == sortedSet.Count ? 0 : 1;
    }
}