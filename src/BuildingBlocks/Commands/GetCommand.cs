using codecrafters_redis.BuildingBlocks;

namespace codecrafters_redis;

public class GetCommand : ICommand
{
    public object[] Arguments { get; set; }
}