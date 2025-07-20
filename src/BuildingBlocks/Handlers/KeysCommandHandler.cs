using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the execution of the "KEYS" command within a Redis-like system.
/// </summary>
/// <remarks>
///     The KeysCommandHandler is responsible for processing the "KEYS" command to retrieve a list
///     of keys stored in the system. It filters the keys to exclude expired ones by utilizing the
///     WatchDog class.
///     Redis link: https://redis.io/docs/latest/commands/keys/
/// </remarks>
public class KeysCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    private readonly WatchDog _watchDog;
    
    public string HandlingCommandName => Constants.KeysCommand;
    
    public KeysCommandHandler(RedisStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();
        if (subCommand == "*")
        {
            var allKeys = _storage.GetAllKeys();
            var aliveKeys = allKeys.Where(key => !_watchDog.IsExpired(key)).Select(key => key).ToArray();

            var responses = aliveKeys.Select(x=> BulkStringResult.Create(x)).ToArray();

            return Task.FromResult<CommandResult>(ArrayResult.Create(responses));
        }
        
        return Task.FromResult<CommandResult>(ErrorResult.Create($"unknown command {subCommand}"));
    }
}