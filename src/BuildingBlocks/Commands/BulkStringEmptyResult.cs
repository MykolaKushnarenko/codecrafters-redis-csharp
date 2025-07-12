namespace codecrafters_redis.BuildingBlocks.Commands;

public class BulkStringEmptyResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.BulkString;
    public string Value => "$-1";
}