using System.Text;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PingCommandHandler : ICommandHandler<PingCommand>
{
    public Task<byte[]> HandleAsync(PingCommand command)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.PongResponse));
    }
}