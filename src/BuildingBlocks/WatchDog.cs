using System.Collections.Concurrent;

namespace codecrafters_redis.BuildingBlocks;

public class WatchDog
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _timestamps = new();
    
    public void Watch(string key, DateTimeOffset timestamp)
    {
        _timestamps[key] = timestamp;
    }

    public bool IsExpired(string key)
    {
        if (_timestamps.TryGetValue(key, out var timestamp))
        {
            return DateTimeOffset.UtcNow > timestamp;
        }

        return false;
    }
}