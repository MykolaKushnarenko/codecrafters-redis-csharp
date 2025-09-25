using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

/// <summary>
///     Handles the execution of the "GET" command for the Redis-like system.
/// </summary>
/// <remarks>
///     The GetCommandHandler is responsible for retrieving the value associated with a
///     given key from the RedisStorage. It interacts with the storage to fetch the value
///     and ensures that the key has not expired based on the WatchDog's validation.
///     If the key is expired, it removes the key from storage and returns an empty bulk string result.
/// </remarks>
public class GetCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    private readonly WatchDog _watchDog;
    
    public GetCommandHandler(RedisStorage storage, WatchDog watchDog)
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

        if (value == RedisValue.Null)
        {
            return Task.FromResult<CommandResult>(new BulkStringEmptyResult());
        }
        
        return Task.FromResult<CommandResult>(BulkStringResult.Create(value.Value.ToString()!));
    }
}