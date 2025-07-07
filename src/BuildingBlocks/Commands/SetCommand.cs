using codecrafters_redis.BuildingBlocks;

namespace codecrafters_redis;

public class SetCommand : ICommand
{
    public object[] Arguments { get; set; }
}