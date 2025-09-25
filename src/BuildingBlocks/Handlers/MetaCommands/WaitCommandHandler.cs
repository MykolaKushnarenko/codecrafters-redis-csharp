using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;

namespace DotRedis.BuildingBlocks.Handlers.MetaCommands;

/// <summary>
///     Handles the "WAIT" command, which waits for replication acknowledgments from a specified
///     number of replicas within a given timeout period.
/// </summary>
/// <remarks>
///     This handler processes commands that require monitoring the state of replication. It waits until
///     the specified number of replicas have acknowledged a write or until the timeout has elapsed,
///     whichever comes first. If no replication has occurred yet, it immediately returns the number of replicas.
///     Redis link: https://redis.io/docs/latest/commands/wait/
/// </remarks>
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