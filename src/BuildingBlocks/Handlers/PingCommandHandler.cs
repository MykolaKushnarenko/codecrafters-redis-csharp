using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Configurations;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the processing of the PING command in the Redis-like system.
/// </summary>
/// <remarks>
///     The PingCommandHandler is responsible for responding to the PING command.
///     If the server is in "slave" mode, it returns a replication-specific response ("MasterReplicationResult").
///     Otherwise, it returns a simple "PONG" response ("SimpleStringResult").
///     Redis link: https://redis.io/docs/latest/commands/ping/
/// </remarks>
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