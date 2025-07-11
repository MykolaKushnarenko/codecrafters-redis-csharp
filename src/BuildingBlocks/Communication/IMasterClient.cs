namespace codecrafters_redis.BuildingBlocks.Communication;

public interface IMasterClient
{
    public Task<CommunicationResult> SendPing(CancellationToken cancellationToken);
    
    public Task<CommunicationResult> SendRepConfigListeningPort(CancellationToken cancellationToken);
    
    public Task<CommunicationResult> SendRepConfigCapa(CancellationToken cancellationToken);
    
    public Task<CommunicationResult> SendPSync(CancellationToken cancellationToken);
}