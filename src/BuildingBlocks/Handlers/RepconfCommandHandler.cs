using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the REPLCONF command within the Redis-like system.
/// </summary>
/// <remarks>
///     RepconfCommandHandler is responsible for executing specific subcommands of the REPLCONF command.
///     These subcommands relate to replication configuration and acknowledgment behaviors in the system.
///     Redis link: https://redis.io/docs/latest/commands/replconf/
/// </remarks>
public class RepconfCommandHandler : ICommandHandler<Command>
{
    private readonly ReplicationManager _replicationManager;
    private readonly AcknowledgeCommandTracker _acknowledge;
    
    public RepconfCommandHandler(ReplicationManager replicationManager, AcknowledgeCommandTracker acknowledge)
    {
        _replicationManager = replicationManager;
        _acknowledge = acknowledge;
    }
    
    public string HandlingCommandName => Constants.RepconfCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handling Repconf Command");
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("listening-port", StringComparison.CurrentCultureIgnoreCase) || 
            subCommand.Equals("capa", StringComparison.CurrentCultureIgnoreCase))
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.OkResponse));
        }

        if (subCommand.Equals("GETACK", StringComparison.CurrentCultureIgnoreCase))
        {
            return Task.FromResult<CommandResult>(ArrayResult.Create(BulkStringResult.Create("REPLCONF"), BulkStringResult.Create("ACK"), BulkStringResult.Create(_acknowledge.TotalProcessedCommandBytes.ToString())));
        }

        if (subCommand.Equals("ACK", StringComparison.CurrentCultureIgnoreCase))
        {
            _replicationManager.MarkReplicaAsSynced();
            return Task.FromResult<CommandResult>(new MasterReplicationResult());
        }
        
        return Task.FromResult<CommandResult>(ErrorResult.Create($"unknown command {subCommand}"));
    }
}