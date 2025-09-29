using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class LPushCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    private readonly RedisValueListener _listener;

    public LPushCommandHandler(
        RedisStorage storage, 
        RedisValueListener listener)
    {
        _storage = storage;
        _listener = listener;
    }

    public string HandlingCommandName => Constants.LPushCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        
        var numberOfItem = 0;
        
        foreach (var argument in command.Arguments.Skip(1))
        {
            var value = argument.ToString();
            numberOfItem = _storage.LPush(key, RedisValue.Create(value));
            _listener.Signal(key);
        }
        
        return Task.FromResult<CommandResult>(IntegerResult.Create(numberOfItem));
    }
}