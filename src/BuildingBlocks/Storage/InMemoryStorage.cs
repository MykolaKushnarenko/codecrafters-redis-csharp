using System.Collections.Concurrent;

namespace codecrafters_redis.BuildingBlocks.Storage;

public class InMemoryStorage
{
    private readonly ConcurrentDictionary<string, object> _storage = new();
    
    public void Set(string key, object value)
    {
        _storage[key] = value;
    }
    
    public object? Get(string key)
    {
        return _storage.GetValueOrDefault(key, null);
    }

    public void Remove(string key)
    {
        _storage.TryRemove(key, out _);
    }

    public string[] GetAllKeys()
    {
        return _storage.Keys.ToArray();
    }
}