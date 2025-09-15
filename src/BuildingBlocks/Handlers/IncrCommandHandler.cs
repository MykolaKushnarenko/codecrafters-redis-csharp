using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

public class IncrCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public IncrCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }
    
    public string HandlingCommandName => Constants.IncrCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var redisValue = _storage.Get(key);

        if (redisValue == RedisValue.Null)
        {
            const long defaultValue = 1; // default value if key does not exist
            _storage.Set(key, RedisValue.Create(defaultValue));
            return Task.FromResult<CommandResult>(IntegerResult.Create(defaultValue));
        }

        if (redisValue.Type != RedisValueType.Integer)
            return Task.FromResult<CommandResult>(ErrorResult.Create("value is not an integer or out of range"));
        
        var value = (long)redisValue.Value;
        value++;
        _storage.Set(key, RedisValue.Create(value));
            
        return Task.FromResult<CommandResult>(IntegerResult.Create(value));
    }
}