using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

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
        
        var numberOfItem = 0;
        
        foreach (var argument in command.Arguments.Skip(1))
        {
            var value = argument.ToString();
            numberOfItem = _storage.Push(key, RedisValue.Create(value));
        }
        
        return Task.FromResult<CommandResult>(IntegerResult.Create(numberOfItem));
    }
}