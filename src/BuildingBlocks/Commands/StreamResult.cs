namespace codecrafters_redis.BuildingBlocks.Commands;

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