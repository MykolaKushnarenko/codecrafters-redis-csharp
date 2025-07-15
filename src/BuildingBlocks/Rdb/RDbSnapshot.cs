namespace codecrafters_redis.BuildingBlocks.Rdb;

public class RDbSnapshot
{
    public string MagicHeader { get; set; }
    
    public int DbNumber { get; set; }
    
    public Dictionary<string, string> Metadata { get; set; } = new();
    
    public Dictionary<string, object> KeyValues { get; set; } = new();

    public Dictionary<string, DateTimeOffset> KeyExpirationTimestamps { get; set; } = new();

}