namespace codecrafters_redis.BuildingBlocks.Commands;

public interface ICommand
{
    object[] Arguments { get; set; }
}