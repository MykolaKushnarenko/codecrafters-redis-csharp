using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class KeysCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    
    public string HandlingCommandName => Constants.KeysCommand;
    
    public KeysCommandHandler(InMemoryStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }
    
    public Task<byte[]> HandleAsync(Command command)
    {
        var subCommand = command.Arguments[0].ToString();
        if (subCommand == "*")
        {
            var sb = new StringBuilder();
            
            var allKeys = _storage.GetAllKeys();
            var aliveKeys = allKeys.Where(key => !_watchDog.IsExpired(key)).Select(key => key).ToArray();

            sb.Append($"*{aliveKeys.Length}{Constants.EOL}");

            foreach (var key in aliveKeys)
            {
                sb.Append($"${key.Length}{Constants.EOL}{key}{Constants.EOL}");
            }
            
            return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));
    }
}