using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers;

public class TransactionCommandHandlerDecorator : ICommandHandler<Command>
{
    private readonly ICommandHandler<Command> _decoratedHandler;
    private readonly TransactionManager _manager;
    
    public TransactionCommandHandlerDecorator(ICommandHandler<Command> decoratedHandler, TransactionManager manager)
    {
        _decoratedHandler = decoratedHandler;
        _manager = manager;
    }

    public string HandlingCommandName => _decoratedHandler.HandlingCommandName;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (_manager.HasStarted)
        {
            var transactionAction = new TransactionAction
            {
                HandlerFunc = _decoratedHandler.HandleAsync,
                Command = command
            };
            
            _manager.AddAction(transactionAction);
            
            return Task.FromResult<CommandResult>(SimpleStringResult.Create("QUEUED"));
        }

        return _decoratedHandler.HandleAsync(command, cancellationToken);
    }
}