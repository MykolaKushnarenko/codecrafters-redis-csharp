using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace DotRedis.BuildingBlocks.HandlerFactory;

/// <summary>
///     Factory class responsible for providing the appropriate command handler
///     for a given command name.
/// </summary>
public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly Dictionary<string, ICommandHandler<Command>> _commandHandlerMap;
    
    public CommandHandlerFactory(IServiceProvider provider)
    {
        var handler = provider.GetServices<ICommandHandler<Command>>();
        _commandHandlerMap = handler.ToDictionary(x => x.HandlingCommandName, x => x);
    }

    public ICommandHandler<Command> GetHandler(string commandName) =>
        _commandHandlerMap!.GetValueOrDefault(commandName, null);
}