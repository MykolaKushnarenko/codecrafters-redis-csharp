using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Handlers;

namespace codecrafters_redis.BuildingBlocks.HandlerFactory;

public interface ICommandHandlerFactory
{
    ICommandHandler<Command>? GetHandler(string commandName);
}