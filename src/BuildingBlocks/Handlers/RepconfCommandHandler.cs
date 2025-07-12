using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class RepconfCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.RepconfCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("listening-port", StringComparison.CurrentCultureIgnoreCase) || 
            subCommand.Equals("capa", StringComparison.CurrentCultureIgnoreCase))
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.OkResponse));
        }
        
        return Task.FromResult<CommandResult>(ErrorResult.Create($"unknown command {subCommand}"));
    }
}