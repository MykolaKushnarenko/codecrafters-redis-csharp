using System.Collections;

namespace codecrafters_redis.BuildingBlocks.Storage;

public class RedisValue
{
    public RedisValueType Type { get; }
    public object Value { get; }

    public static RedisValue Create(object value)
    {
        RedisValue result = value switch
        {
            string s => new RedisValue(RedisValueType.String, s),
            IList list => new RedisValue(RedisValueType.List, list),
            IDictionary dictionary => new RedisValue(RedisValueType.Hash, dictionary),
            long integer => new RedisValue(RedisValueType.Integer, integer),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };

        return result;
    }
    
    public static RedisValue Stream(RedisStream stream) => new(RedisValueType.Stream, stream);

    public static readonly RedisValue Null = new();

    private RedisValue()
    {
    }
    
    private RedisValue(RedisValueType type, object value)
    {
        Type = type;
        Value = value;
    }
}