using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommnds;

public class RPushCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public RPushCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.RPushCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var value = command.Arguments[1].ToString();
        
        var numberOfItem = _storage.Push(key, RedisValue.Create(value));
        
        return Task.FromResult<CommandResult>(IntegerResult.Create(numberOfItem));
    }
}