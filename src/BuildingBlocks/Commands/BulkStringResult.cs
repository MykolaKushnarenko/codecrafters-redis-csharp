namespace codecrafters_redis.BuildingBlocks.Commands;

public class BulkStringResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.BulkString;
    public string[] Value { get; }

    private BulkStringResult(string[] value)
    {
        Value = value;
    }
    
    public static BulkStringResult Create(params string[] message)
    {
        return new BulkStringResult(message);
    }
}