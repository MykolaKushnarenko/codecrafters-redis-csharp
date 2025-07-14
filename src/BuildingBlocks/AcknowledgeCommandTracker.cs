namespace codecrafters_redis.BuildingBlocks;

public class AcknowledgeCommandTracker
{
    private long _totalProcessedCommandBytes;
    
    public long TotalProcessedCommandBytes => _totalProcessedCommandBytes;
    
    public void AddProcessedCommandBytes(long bytesRead)
    {
        Interlocked.Add(ref _totalProcessedCommandBytes, bytesRead);
    }
}