using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Handlers;

namespace DotRedis.BuildingBlocks.HandlerFactory;

public interface ICommandHandlerFactory
{
    ICommandHandler<Command>? GetHandler(string commandName);
}