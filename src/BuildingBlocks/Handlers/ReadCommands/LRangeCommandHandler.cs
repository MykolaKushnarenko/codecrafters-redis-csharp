using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

public class LRangeCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    
    public LRangeCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }
    
    public string HandlingCommandName => Constants.LRangeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var start = int.Parse(command.Arguments[1].ToString());
        var end = int.Parse(command.Arguments[2].ToString()); // +1 to include the last element in the range;

        if (start > end)
        {
            return Task.FromResult<CommandResult>(ArrayResult.Create());
        }
        
        var redisValue = _storage.Get(key);
        if (redisValue == RedisValue.Null || redisValue.Type != RedisValueType.List)
        {
            return Task.FromResult<CommandResult>(ArrayResult.Create());
        }
        
        var list = (List<RedisValue>)redisValue.Value;

        var commandResults = new List<BulkStringResult>();

        if (end > list.Count)
        {
            end = list.Count - 1;
        }
        
        for (var i = start; i <= end; i++)
        {
            var item = list[i];
            commandResults.Add(BulkStringResult.Create(item.Value.ToString()));
        }
        
        return Task.FromResult<CommandResult>(ArrayResult.Create(commandResults.ToArray()));
    }
}