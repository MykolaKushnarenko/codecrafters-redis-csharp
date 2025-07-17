namespace codecrafters_redis.BuildingBlocks.Exceptions;

public class RedisException : Exception
{
    public RedisException(string message) : base(message)
    {
    }
}