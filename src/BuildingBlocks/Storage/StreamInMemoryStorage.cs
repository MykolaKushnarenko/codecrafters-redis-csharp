using System.Collections.Concurrent;

namespace codecrafters_redis.BuildingBlocks.Storage;

public class StreamInMemoryStorage
{
    
    // reimplement it to radix trees
    private readonly ConcurrentDictionary<string, RStream> _storage = new();

    public void AddValue(string streamId, string id, string key, object value)
    {
        if(!_storage.TryGetValue(streamId, out _))
        {
            _storage.TryAdd(streamId, new RStream
            {
                Id = id,
                KeyValuePairs = new Dictionary<string, object>()
            });
        }
        
        _storage[streamId].KeyValuePairs.Add(key, value);
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