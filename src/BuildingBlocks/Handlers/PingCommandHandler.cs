using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PingCommandHandler : ICommandHandler<Command>
{
    private readonly ServerConfiguration _configuration;

    public PingCommandHandler(ServerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HandlingCommandName => Constants.PingCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (_configuration.Role == "slave")
        {
            return Task.FromResult<CommandResult>(new MasterReplicationResult());
        }
        
        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.PongResponse));
    }
}