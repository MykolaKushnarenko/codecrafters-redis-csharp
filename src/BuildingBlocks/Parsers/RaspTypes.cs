namespace DotRedis.BuildingBlocks.Parsers;

/// <summary>
///     A static class that defines constant byte values representing the different data types
///     used in the Redis Serialization Protocol (RESP).
/// </summary>
public static class RaspTypes
{
    public const byte SimpleStrings = (byte)'+';
    public const byte Erorrs = (byte)'-';
    public const byte Integers = (byte)':';
    public const byte BulkStrings = (byte)'$';
    public const byte Arrays = (byte)'*';
}