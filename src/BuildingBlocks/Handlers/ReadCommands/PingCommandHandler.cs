using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Configurations;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

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
    private readonly SubscriptionManager _subscriptionManager;

    public PingCommandHandler(
        ServerConfiguration configuration, 
        SubscriptionManager subscriptionManager)
    {
        _configuration = configuration;
        _subscriptionManager = subscriptionManager;
    }

    public string HandlingCommandName => Constants.PingCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (_configuration.Role == "slave")
        {
            return Task.FromResult<CommandResult>(new MasterReplicationResult());
        }

        if (_subscriptionManager.HasAnySubscription)
        {
            var arrayResult = ArrayResult.Create(BulkStringResult.Create(Constants.PongResponse.ToLower()));
            arrayResult.Add(BulkStringResult.Create(string.Empty));

            return Task.FromResult<CommandResult>(arrayResult);
        }
        
        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.PongResponse));
    }
}