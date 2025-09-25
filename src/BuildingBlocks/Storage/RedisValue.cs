using System.Collections;
using DotRedis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Storage;

public class RedisValue
{
    public static readonly RedisValue Null = new();
    
    private RedisValue()
    {
    }
    
    private RedisValue(RedisValueType type, object value)
    {
        Type = type;
        Value = value;
    }
    
    public RedisValueType Type { get; }
    public object Value { get; set; }

    public static RedisValue Create(object value)
    {
        var result = value switch
        {
            IList list => new RedisValue(RedisValueType.List, list),
            IDictionary dictionary => new RedisValue(RedisValueType.Hash, dictionary),
            long integer => new RedisValue(RedisValueType.Integer, integer),
            string s => CreateRedisValue(s),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };

        return result;
    }

    private static RedisValue CreateRedisValue(string s)
    {
        if (long.TryParse(s, out var l))
        {
            return new RedisValue(RedisValueType.Integer, l);
        }
        
        return new RedisValue(RedisValueType.String, s);
    }
}