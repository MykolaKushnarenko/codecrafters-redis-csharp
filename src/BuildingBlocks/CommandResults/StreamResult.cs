namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a command result type specifically for streaming responses.
///     This is a custom model that represents streams.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/develop/data-types/streams/
///
///     This class might be redundant and will potentially be removed in the future since <see cref="ArrayResult"/> may suffice.
/// </remarks>
public class StreamResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.Stream;
    public IEnumerable<CommandResult> Stream { get; }

    private StreamResult(IEnumerable<CommandResult> stream)
    {
        Stream = stream;
    }
    
    public static StreamResult Create(IEnumerable<CommandResult> stream)
    {
        return new StreamResult(stream);
    }
}