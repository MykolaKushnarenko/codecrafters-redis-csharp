using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PingCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.PingCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.PongResponse));
    }
}