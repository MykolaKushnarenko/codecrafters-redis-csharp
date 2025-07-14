using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class WaitCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.WaitCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(IntegerResult.Create(0));
    }
}