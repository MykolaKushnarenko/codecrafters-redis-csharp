namespace DotRedis.BuildingBlocks.Parsers;

/// <summary>
///     Represents a parsed protocol command with its associated metadata.
/// </summary>
/// <remarks>
///     This class is used within the Redis-like protocol parsing and handling system.
/// </remarks>
public class RaspProtocolData
{
    /// <summary>
    ///     Represents the name of the command parsed from the protocol input. E.g. GET, SET, WAIT...
    /// </summary>
    /// <remarks>
    ///     Redis link: https://redis.io/docs/latest/commands/
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the arguments associated with a command.
    /// </summary>
    /// <remarks>
    ///     Example is "capa", "psync2" for REPLCONF command
    /// </remarks>
    public object[] Arguments { get; set; }

    /// <summary>
    ///     Represents the total size, in bytes, of the command received or processed.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the length of the command in bytes,
    ///     typically calculated from the raw input data. It is useful for
    ///     tracking the size of incoming commands for replication
    /// </remarks>
    public long CommandByteLength { get; set; }
}