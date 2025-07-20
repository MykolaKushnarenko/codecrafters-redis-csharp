namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a command result specifically associated with master replication.
/// </summary>
/// <remarks>
///     This class is used to signify a replication-related response in the context of a Redis master-replica setup.
///     Also, this class represent no response since in some cases replica will not return anything.
/// </remarks>
public class MasterReplicationResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.MasterReplication;
}