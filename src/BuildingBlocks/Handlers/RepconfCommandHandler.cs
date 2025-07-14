using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class RepconfCommandHandler : ICommandHandler<Command>
{
    private readonly ReplicationManager _replicationManager;
    private readonly ServerConfiguration _configuration;
    private readonly AcknowledgeCommandTracker _acknowledge;
    
    public RepconfCommandHandler(ReplicationManager replicationManager, ServerConfiguration configuration, AcknowledgeCommandTracker acknowledge)
    {
        _replicationManager = replicationManager;
        _configuration = configuration;
        _acknowledge = acknowledge;
    }
    
    public string HandlingCommandName => Constants.RepconfCommand;
    
    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handling Repconf Command");
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("listening-port", StringComparison.CurrentCultureIgnoreCase) || 
            subCommand.Equals("capa", StringComparison.CurrentCultureIgnoreCase))
        {
            return SimpleStringResult.Create(Constants.OkResponse);
        }

        if (subCommand.Equals("GETACK", StringComparison.CurrentCultureIgnoreCase))
        {
            return ArrayResult.Create(BulkStringResult.Create("REPLCONF"), BulkStringResult.Create("ACK"), BulkStringResult.Create(_acknowledge.TotalProcessedCommandBytes.ToString()));
        }

        if (subCommand.Equals("ACK", StringComparison.CurrentCultureIgnoreCase))
        {
            _replicationManager.MarkReplicaAsSynced();
            return new MasterReplicationResult();
        }
        
        return ErrorResult.Create($"unknown command {subCommand}");
    }
}