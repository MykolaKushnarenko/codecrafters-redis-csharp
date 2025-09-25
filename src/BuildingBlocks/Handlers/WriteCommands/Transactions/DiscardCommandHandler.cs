using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands.Transactions;

public class DiscardCommandHandler : ICommandHandler<Command>
{
    private readonly TransactionManager _transactionManager;
    
    public DiscardCommandHandler(TransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }
    
    public string HandlingCommandName => Constants.DiscardCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (!_transactionManager.HasStarted)
        {
            return Task.FromResult<CommandResult>(ErrorResult.Create("DISCARD without MULTI"));
        }
        
        _transactionManager.Discard();
        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.OkResponse));
    }
}