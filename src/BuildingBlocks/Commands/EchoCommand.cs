using codecrafters_redis.BuildingBlocks;

namespace codecrafters_redis;

public class EchoCommand : ICommand
{
    public object[] Arguments { get; set; }
}