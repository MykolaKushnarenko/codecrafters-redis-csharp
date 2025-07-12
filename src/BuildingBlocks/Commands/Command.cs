namespace codecrafters_redis.BuildingBlocks.Commands;

public class Command : ICommand
{
    public byte[] InitialCommandBytes { get; set; }
    
    public object[] Arguments { get; set; }
}