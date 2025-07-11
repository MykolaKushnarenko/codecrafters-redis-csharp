namespace codecrafters_redis.BuildingBlocks.Communication;

public interface IMasterClient
{
    public Task<CommunicationResult> Ping();
}