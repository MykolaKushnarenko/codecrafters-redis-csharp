namespace codecrafters_redis.BuildingBlocks.Commands;

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