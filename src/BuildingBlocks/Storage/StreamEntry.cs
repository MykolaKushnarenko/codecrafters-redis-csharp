using codecrafters_redis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Storage;

//TODO: merge with RedisValue class
public class StreamEntry
{
    public string Id { get; }
    public Dictionary<string, RedisValue> Fields { get; }
    public DateTimeOffset Timestamp { get; }

    public StreamEntry(string id, Dictionary<string, RedisValue> fields)
    {
        Id = id;
        Fields = fields;
        Timestamp = ParseTimestampFromId(id);
    }

    private DateTimeOffset ParseTimestampFromId(string id)
    {
        var parts = id.Split('-');
        if (parts.Length != 2) 
            throw new ArgumentException("Invalid stream ID format");
        
        return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parts[0]));
    }
}