namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a CommandResult of type BulkString.
/// </summary>
/// <remarks>
///     This class is a specific implementation of the abstract CommandResult type,
///     where the result corresponds to a BulkString in the Redis serialization protocol (RESP).
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#bulk-strings
/// </remarks>
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