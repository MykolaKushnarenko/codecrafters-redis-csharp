using codecrafters_redis.BuildingBlocks;

namespace codecrafters_redis;

public class PingCommand : ICommand
{
    public object[] Arguments { get; set; }
}