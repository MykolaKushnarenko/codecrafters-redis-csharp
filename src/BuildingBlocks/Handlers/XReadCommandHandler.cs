using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

public class XReadCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    
    public XReadCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }
    
    public string HandlingCommandName => Constants.XReadCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();
        var key = command.Arguments[1].ToString();
        var idVal = command.Arguments[2].ToString();

        var stream = _storage.GetStream(key);

        Console.WriteLine(idVal);
        var entries = stream.Read(idVal);
        Console.WriteLine(entries.Length);
        
        var list = new List<ArrayResult>();
        
        foreach (var entry in entries)
        {
            var id = BulkStringResult.Create(entry.Id);
            var fields = new List<BulkStringResult>();
            Console.WriteLine(entry.Fields.Count);
            foreach (var field in entry.Fields)
            {
                var redisValue = field.Value;
                
                fields.Add(BulkStringResult.Create(field.Key));
                fields.Add(BulkStringResult.Create(redisValue.Value.ToString()));
            }
            
            list.Add(ArrayResult.Create(id, ArrayResult.Create(fields.ToArray())));
        }
        
        var streamArrayResult = ArrayResult.Create(list.ToArray());
        var streamResult = ArrayResult.Create(BulkStringResult.Create(key), streamArrayResult);
        
        return Task.FromResult<CommandResult>(ArrayResult.Create(streamResult));
    }
}