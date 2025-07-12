using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class EchoCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.EchoCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(BulkStringResult.Create(command.Arguments[0].ToString()));
    }
}