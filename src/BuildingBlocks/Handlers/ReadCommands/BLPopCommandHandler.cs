using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

public class BLPopCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    private readonly RedisValueListener _listener;
    
    public BLPopCommandHandler(RedisStorage storage, RedisValueListener listener)
    {
        _storage = storage;
        _listener = listener;
    }
    
    public string HandlingCommandName => Constants.BLPopCommand;
    
    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();

        var priorRedisValue = _storage.Get(key); 
        
        List<RedisValue> priorList;
        if (priorRedisValue == RedisValue.Null)
        {
            priorList = [];
        }
        else
        {
            priorList = (List<RedisValue>)priorRedisValue.Value;
        }
        
        var timeToWait = int.Parse(command.Arguments[1].ToString());

        if (timeToWait == 0)
        {
            await _listener.WaitForNewDataAsync(key);
        }
        else
        {
            await Task.Delay(timeToWait, cancellationToken);
        }
        
        var currentRedisValue = _storage.Get(key);
        var currentList = (List<RedisValue>)currentRedisValue.Value;

        var diff = currentList.Except(priorList).ToList();

        var result = new List<CommandResult>();

        foreach (var item in diff)
        {
            result.Add(BulkStringResult.Create(item.Value.ToString()));
        }
        
        var arrayResult = ArrayResult.Create(BulkStringResult.Create(key));
        arrayResult.Add(result.ToArray());

        return arrayResult;
    }
}