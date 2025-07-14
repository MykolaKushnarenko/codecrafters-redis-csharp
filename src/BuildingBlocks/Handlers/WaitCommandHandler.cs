using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class WaitCommandHandler : ICommandHandler<Command>
{
    private readonly ReplicationManager _replicationManager;

    public WaitCommandHandler(ReplicationManager replicationManager)
    {
        _replicationManager = replicationManager;
    }

    public string HandlingCommandName => Constants.WaitCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(IntegerResult.Create(_replicationManager.NumberOfReplicas));
    }
}