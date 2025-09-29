namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a specific type of command result, which is an array of other command results.
///     This will be translated into RESP2 protocol responses.
/// </summary>
/// <remarks>
/// Redis doc: https://redis.io/docs/latest/develop/reference/protocol-spec/#arrays
/// </remarks>
public class ArrayResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.Array;

    /// <summary>
    /// Represents a collection of CommandResult instances contained within the ArrayResult.
    /// </summary>
    /// <remarks>
    /// This property stores the individual elements that make up the array-based command result.
    /// Each item in the list is an instance of the CommandResult class, which can represent various types of responses.
    /// </remarks>
    public List<CommandResult> Items { get; }
    
    private ArrayResult(params CommandResult[] items)
    {
        Items = new List<CommandResult>(items);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ArrayResult"/> class containing the specified items.
    /// </summary>
    /// <param name="items">An array of <see cref="CommandResult"/> objects to include in the <see cref="ArrayResult"/>.</param>
    /// <returns>A new <see cref="ArrayResult"/> containing the provided items.</returns>
    public static ArrayResult Create(params CommandResult[] items)
    {
        return new ArrayResult(items);
    }
    
    public void Add(params CommandResult[] items)
    {
        Items.AddRange(items);
    }
}