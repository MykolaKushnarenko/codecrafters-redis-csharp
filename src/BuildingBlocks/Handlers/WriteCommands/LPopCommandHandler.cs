using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;
using static DotRedis.BuildingBlocks.Constants;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class LPopCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    
    public LPopCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }
    
    public string HandlingCommandName => LPopCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        
        var redisValue = _storage.Get(key);
        if (redisValue == RedisValue.Null || redisValue.Type != RedisValueType.List)
        {
            return Task.FromResult<CommandResult>(new BulkStringEmptyResult());
        }
        
        var list = (List<RedisValue>)redisValue.Value;

        if (list.Count == 0)
        {
            return Task.FromResult<CommandResult>(new BulkStringEmptyResult());
        }
        
        var first = list.FirstOrDefault();
        list.RemoveAt(0);
        
        return Task.FromResult<CommandResult>(BulkStringResult.Create(first.Value.ToString()));
    }
}