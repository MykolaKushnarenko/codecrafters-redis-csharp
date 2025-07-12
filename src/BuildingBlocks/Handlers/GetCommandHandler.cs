using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class GetCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    
    public GetCommandHandler(InMemoryStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }

    public string HandlingCommandName => Constants.GetCommand;

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString(); 
        
        if (_watchDog.IsExpired(key!))
        {
            _storage.Remove(key!);
            return Task.FromResult<CommandResult>(new BulkStringEmptyResult());
        }
        
        var value = _storage.Get(key!);

        return Task.FromResult<CommandResult>(BulkStringResult.Create(value.ToString()!));
    }
}