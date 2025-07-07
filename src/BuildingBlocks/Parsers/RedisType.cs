namespace codecrafters_redis.BuildingBlocks.Parsers;

public static class RedisType
{
    public const byte SimpleStrings = (byte)'+';
    public const byte Erorrs = (byte)'-';
    public const byte Integers = (byte)':';
    public const byte BulkStrings = (byte)'$';
    public const byte Arrays = (byte)'*';
}