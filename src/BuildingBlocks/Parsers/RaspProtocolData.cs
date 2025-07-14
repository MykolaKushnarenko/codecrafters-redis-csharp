namespace codecrafters_redis.BuildingBlocks.Parsers;

public class RaspProtocolData
{
    public string Name { get; set; }
    public object[] Arguments { get; set; }
    
    public long CommandByteLength { get; set; }
}