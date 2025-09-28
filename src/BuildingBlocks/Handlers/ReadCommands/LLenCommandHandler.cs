using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

public class LLenCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public LLenCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }


    public string HandlingCommandName => Constants.LLenCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();

        var redisValue = _storage.Get(key);
        if (redisValue == RedisValue.Null || redisValue.Type != RedisValueType.List)
        {
            return Task.FromResult<CommandResult>(IntegerResult.Create(0));
        }
        
        var list = (List<RedisValue>)redisValue.Value;
        return Task.FromResult<CommandResult>(IntegerResult.Create(list.Count));
    }
}