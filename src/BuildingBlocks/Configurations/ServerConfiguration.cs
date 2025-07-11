namespace codecrafters_redis.BuildingBlocks.Configurations;

public class ServerConfiguration
{
    public string Dir { get; set; }
    
    public string DbFileName { get; set; }

    public int Port { get; set; } = 6379;
    
    public string Role { get; set; } = "master";
    
    public string MasterHost { get; set; }
    
    public int MasterPort { get; set; }
}