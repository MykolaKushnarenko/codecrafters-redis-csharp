using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.HandlerFactory;

public interface ICommandHandlerFactory
{
    ICommandHandler<Command>? GetHandler(string commandName);
}