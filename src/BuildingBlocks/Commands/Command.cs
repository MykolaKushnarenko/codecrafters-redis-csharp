namespace codecrafters_redis.BuildingBlocks.Commands;

public class Command : ICommand
{
    public object[] Arguments { get; set; }
}