namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents the result of a null array command in Redis when the value is empty or does not exist.
/// </summary>
/// <remarks>
///     This class is used in scenarios where a null array result is expected,
///     but the returned value is an empty result, indicated by "*-1" in the RESP protocol.
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#null-arrays
/// </remarks>
public class ArrayEmptyResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.NullArray;

    /// <summary>
    ///     Represents the string value associated with a null array,
    ///     indicating an empty bulk string response format in Redis.
    /// </summary>
    /// <remarks>
    ///     The value is defined as "*-1", which is used by Redis to denote
    ///     an absence of a bulk string result.
    /// </remarks>
    public string Value => "*-1";
}