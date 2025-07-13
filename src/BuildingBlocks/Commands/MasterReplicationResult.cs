namespace codecrafters_redis.BuildingBlocks.Commands;

public class MasterReplicationResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.MasterReplication;
}