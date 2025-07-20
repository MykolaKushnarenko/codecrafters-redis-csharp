namespace DotRedis.BuildingBlocks.Commands;

/// <summary>
///     Represents a command to be processed within the Redis-like system.
/// </summary>
public class Command : ICommand
{
    /// <summary>
    ///     Gets or sets the total byte length of the command, including its arguments.
    /// </summary>
    /// <remarks>
    /// This property is used to track the size of the command for purposes such as replication.
    /// It represents the total number of bytes consumed by the command,
    /// including its name and all its arguments.
    /// </remarks>
    public long CommandByteLength { get; set; }

    /// <summary>
    ///     Gets or sets the collection of arguments associated with the command.
    /// </summary>
    /// <remarks>
    /// This property represents the arguments that accompany the command
    /// to provide additional context or instruction for execution.
    /// The arguments are processed in the specified order by command handlers.
    /// </remarks>
    public object[] Arguments { get; set; }
}