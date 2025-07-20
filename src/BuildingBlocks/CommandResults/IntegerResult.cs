namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a command result that holds an integer value in the result set.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#integers
/// </remarks>
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