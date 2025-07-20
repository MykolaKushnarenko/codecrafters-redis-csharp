using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.HandlerFactory;
using DotRedis.BuildingBlocks.Parsers;

namespace DotRedis.BuildingBlocks;

/// <summary>
///     The Mediator class serves as the central coordination point for the processing of commands.
///     It interacts with ICommandHandlerFactory to delegate commands to their respective handlers.
/// </summary>
public class Mediator : IMediator
{
    private readonly ICommandHandlerFactory _commandHandlerFactory;
    
    public Mediator(ICommandHandlerFactory commandHandlerFactory)
    {
        _commandHandlerFactory = commandHandlerFactory;
    }

    public async Task<CommandResult> ProcessAsync(RaspProtocolData raspProtocol, CancellationToken cancellationToken)
    {
        var handler = _commandHandlerFactory.GetHandler(raspProtocol.Name);
        
        if (handler == null) return ErrorResult.Create($"unknown command {raspProtocol.Name}");
        
        var command = new Command { Arguments = raspProtocol.Arguments, CommandByteLength = raspProtocol.CommandByteLength};
        return await handler.HandleAsync(command, cancellationToken);
    }
}