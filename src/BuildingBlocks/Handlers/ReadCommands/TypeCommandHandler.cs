using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

/// <summary>
///     Handles the execution of the "TYPE" command in a Redis-like system,
///     which determines the data type of the value stored at a specified key.
/// </summary>
public class TypeCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public TypeCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.TypeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();

        var value = _storage.Get(key);

        if (value != RedisValue.Null)
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create(value.Type.ToString().ToLowerInvariant()));
        }

        var stream = _storage.GetStream(key);

        if (stream != null)
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create("stream"));       
        }
        
        return Task.FromResult<CommandResult>(
            SimpleStringResult.Create("none"));
    }
}