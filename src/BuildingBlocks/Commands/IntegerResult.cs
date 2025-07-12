namespace codecrafters_redis.BuildingBlocks.Commands;

public class IntegerResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.Integer;
    public long Value { get; }

    private IntegerResult(long value)
    {
        Value = value;
    }
    
    public static IntegerResult Create(long message)
    {
        return new IntegerResult(message);
    }
}