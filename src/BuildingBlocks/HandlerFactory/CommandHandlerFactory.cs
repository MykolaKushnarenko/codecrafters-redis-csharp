using codecrafters_redis.BuildingBlocks.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace codecrafters_redis.BuildingBlocks.HandlerFactory;

public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly Dictionary<string, ICommandHandler<Command>> commandHandlerMap;
    
    public CommandHandlerFactory(IServiceProvider provider)
    {
        var handler = provider.GetServices<ICommandHandler<Command>>();
        commandHandlerMap = handler.ToDictionary(x => x.HandlingCommandName, x => x);
    }

    public ICommandHandler<Command>? GetHandler(string commandName) =>
        commandHandlerMap!.GetValueOrDefault(commandName, null);
}