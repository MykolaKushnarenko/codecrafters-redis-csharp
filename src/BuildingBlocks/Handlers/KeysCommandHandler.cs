using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

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