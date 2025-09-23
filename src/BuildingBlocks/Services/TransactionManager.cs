using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.HandlerFactory;

namespace DotRedis.BuildingBlocks.Services;

public class TransactionManager
{
    private readonly AsyncLocal<Transaction> _context = new AsyncLocal<Transaction>();

    public TransactionManager()
    {
        _context.Value = new Transaction();
    }
    
    public async Task<CommandResult> CommitAsync(CancellationToken cancellationToken)
    {
        List<CommandResult> results = [];
        var transaction = _context.Value;
        foreach (var action in transaction.Actions)
        {
             var handleResult = await action.ExecuteAsync(cancellationToken);
             results.Add(handleResult);
        }
        
        return ArrayResult.Create(results.ToArray());
    }

    public void BeginTransaction()
    {
        _context.Value.HasStarted = true;
    }

    public void AddAction(TransactionAction action)
    {
        _context.Value.Actions.Add(action);
    }
    
    public bool HasStarted => _context.Value.HasStarted;
}


public class Transaction
{
    public bool HasStarted { get; set; }
    
    public List<TransactionAction> Actions { get; set; } = [];
}

public class TransactionAction
{
    public Func<Command, CancellationToken, Task<CommandResult>> HandlerFunc { get; set; }
    
    public Command Command { get; set; }

    public Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        return HandlerFunc(Command, cancellationToken);
    }
}