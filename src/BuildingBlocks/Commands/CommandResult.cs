namespace codecrafters_redis.BuildingBlocks.Commands;

public abstract class CommandResult
{
    public abstract CommandResultType Type { get; }
}