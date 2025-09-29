using System.Collections.Concurrent;

namespace DotRedis.BuildingBlocks.Services;

public class RedisValueListener
{
    // potensial problem. 2 clients cannot wait for one stream.
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _taskListenerSources = new();

    public Task WaitForNewDataAsync(string streamName)
    {
        var task = new TaskCompletionSource();
        
        _taskListenerSources.TryAdd(streamName, task);

        return task.Task;
    }
    
    public void Signal(string streamName)
    {
        if (!_taskListenerSources.TryGetValue(streamName, out var task)) return;
        
        task.TrySetResult();
        _taskListenerSources.TryRemove(streamName, out _);
    }
}