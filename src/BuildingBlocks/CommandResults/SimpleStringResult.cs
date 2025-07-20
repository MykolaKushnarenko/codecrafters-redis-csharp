namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a command result of type SimpleString, commonly used to indicate
///     simple responses like "OK" or "PONG" in Redis protocol.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#simple-strings
/// </remarks>
public class SimpleStringResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.SimpleString;
    public string Message { get; }

    private SimpleStringResult(string message)
    {
        Message = message;
    }

    public static SimpleStringResult Create(string message)
    {
        return new SimpleStringResult(message);
    }
}