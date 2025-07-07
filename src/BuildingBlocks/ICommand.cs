namespace codecrafters_redis.BuildingBlocks;

public interface ICommand
{
    object[] Arguments { get; set; }
}