namespace codecrafters_redis.BuildingBlocks.Parsers;

public class RaspProtocolData
{
    public string Name { get; set; }
    public object[] Arguments { get; set; }
    
    public int Length { get; set; }
}