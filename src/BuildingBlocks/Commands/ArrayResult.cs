namespace codecrafters_redis.BuildingBlocks.Commands;

public class ArrayResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.Array;
    public List<CommandResult> Items { get; }

    private ArrayResult(params CommandResult[] items)
    {
        Items = new List<CommandResult>(items);
    }
    
    public static ArrayResult Create(CommandResult[] items)
    {
        return new ArrayResult(items);
    }
}