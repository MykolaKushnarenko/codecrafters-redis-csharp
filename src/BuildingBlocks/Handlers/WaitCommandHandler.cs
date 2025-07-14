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

    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var numberOfReplicatesToWaitFor = int.Parse(command.Arguments[0].ToString());
        var ms = int.Parse(command.Arguments[1].ToString());
        var dateTimeOffsetWait = DateTimeOffset.UtcNow.AddMilliseconds(ms);

        if (_replicationManager.WriteCommandOffset == 0)
        {
            return IntegerResult.Create(_replicationManager.NumberOfReplicas);
        }

        await _replicationManager.GetAcksAsync(cancellationToken);

        while (DateTimeOffset.UtcNow < dateTimeOffsetWait)
        {
            if (_replicationManager.SyncedReplicasCount >= numberOfReplicatesToWaitFor)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
        }

        return IntegerResult.Create(_replicationManager.SyncedReplicasCount);
    }
}