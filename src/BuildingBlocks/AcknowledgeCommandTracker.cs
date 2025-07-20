namespace DotRedis.BuildingBlocks;

/// <summary>
///     Tracks and accumulates the total number of bytes processed by commands in the system.
/// </summary>
public class AcknowledgeCommandTracker
{
    private long _totalProcessedCommandBytes;
    
    public long TotalProcessedCommandBytes => _totalProcessedCommandBytes;
    
    public void AddProcessedCommandBytes(long bytesRead)
    {
        Interlocked.Add(ref _totalProcessedCommandBytes, bytesRead);
    }
}