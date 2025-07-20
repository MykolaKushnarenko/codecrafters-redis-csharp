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
}