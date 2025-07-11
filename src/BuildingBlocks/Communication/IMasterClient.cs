using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Communication;

public interface IMasterClient
{
    public Task<CommunicationResult> Ping(CancellationToken cancellationToken);
    
    public Task<CommunicationResult> RepConfigListeningPort(CancellationToken cancellationToken);
    
    public Task<CommunicationResult> RepConfigCapa(CancellationToken cancellationToken);
}