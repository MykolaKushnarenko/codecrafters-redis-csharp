namespace codecrafters_redis.BuildingBlocks.Commands;

public class Command : ICommand
{
    public long CommandByteLength { get; set; }
    
    public object[] Arguments { get; set; }
}