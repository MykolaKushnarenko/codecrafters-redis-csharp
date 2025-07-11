using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PingCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.PingCommand;
    
    public Task<byte[]> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.PongResponse));
    }
}