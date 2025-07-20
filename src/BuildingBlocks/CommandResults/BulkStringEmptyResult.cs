namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents the result of a bulk string command in Redis when the value is empty or does not exist.
/// </summary>
/// <remarks>
///     This class is used in scenarios where a bulk string result is expected,
///     but the returned value is an empty result, indicated by "$-1" in the RESP protocol.
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#bulk-strings
/// </remarks>
public class BulkStringEmptyResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.BulkString;

    /// <summary>
    ///     Represents the string value associated with a BulkStringEmptyResult,
    ///     indicating an empty bulk string response format in Redis.
    /// </summary>
    /// <remarks>
    ///     The value is defined as "$-1", which is used by Redis to denote
    ///     an absence of a bulk string result.
    /// </remarks>
    public string Value => "$-1";
}