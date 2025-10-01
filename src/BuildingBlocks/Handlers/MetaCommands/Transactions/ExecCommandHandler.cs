using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.MetaCommands.Transactions;

public class ExecCommandHandler : ICommandHandler<Command>
{
    private readonly TransactionManager _transactionManager;

    public ExecCommandHandler(TransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public string HandlingCommandName => Constants.ExecCommand;
    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (_transactionManager.HasStarted)
        {
            return await _transactionManager.CommitAsync(cancellationToken);
        }
        else
        {
            return ErrorResult.Create("EXEC without MULTI");           
        }
    }
}