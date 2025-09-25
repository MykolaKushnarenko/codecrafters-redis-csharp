using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands.Transactions;

public class MultiCommandHandler : ICommandHandler<Command>
{
    private readonly TransactionManager _transactionManager;
    
    public MultiCommandHandler(TransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }
    
    public string HandlingCommandName => Constants.MultiCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        _transactionManager.BeginTransaction();

        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.OkResponse));
    }
}