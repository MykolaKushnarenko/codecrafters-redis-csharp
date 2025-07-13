using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.HandlerFactory;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

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
        
        var command = new Command { Arguments = raspProtocol.Arguments };
        return await handler.HandleAsync(command, cancellationToken);
    }
}