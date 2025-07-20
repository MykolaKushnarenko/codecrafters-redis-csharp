using System.Text;
using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Exceptions;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

public class XRangeCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public XRangeCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.XRangeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var start = command.Arguments[1].ToString();
        var end = command.Arguments[2].ToString();
        
        var stream = _storage.GetStream(key);
        
        var entries = stream.Range(start, end);
        
        var list = new List<ArrayResult>();

        foreach (var entry in entries)
        {
            var id = BulkStringResult.Create(entry.Id);
            var fields = new List<BulkStringResult>();
            foreach (var field in entry.Fields)
            {
                var redisValue = field.Value as RedisValue;
                
                fields.Add(BulkStringResult.Create(field.Key));
                fields.Add(BulkStringResult.Create(redisValue.Value.ToString()));
            }
            
            list.Add(ArrayResult.Create(id, ArrayResult.Create(fields.ToArray())));
        }

        return Task.FromResult<CommandResult>(ArrayResult.Create(list.ToArray()));
    }
}