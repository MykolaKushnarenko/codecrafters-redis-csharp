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

        var count = 1;
        
        if (command.Arguments.Length > 1)
        {
            count = int.Parse(command.Arguments[1].ToString());
        }
        
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

        if (count > list.Count)
        {
            count = list.Count;
        }
        
        var result = new List<BulkStringResult>();
        
        for (var i = 0; i < count; i++)
        {
            var first = list.FirstOrDefault();
            list.RemoveAt(0);
            result.Add(BulkStringResult.Create(first.Value.ToString()));
        }
        
        return Task.FromResult<CommandResult>(result.Count == 1 ? result[0] : ArrayResult.Create(result.ToArray()));
    }
}