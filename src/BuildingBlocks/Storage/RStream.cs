namespace codecrafters_redis.BuildingBlocks.Storage;

public class RStream
{
    public string Id { get; set; }
    
    public Dictionary<string, object> KeyValuePairs { get; set; }
}