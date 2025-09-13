using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Exceptions;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the execution of the "XADD" command for a Redis-like system.
///     This command is used to append a new entry to a stream data structure in the database.
/// </summary>
public class XAddCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;
    private readonly RedisStreamListener _listener;

    public XAddCommandHandler(RedisStorage storage, RedisStreamListener listener)
    {
        _storage = storage;
        _listener = listener;
    }

    public string HandlingCommandName => Constants.XAddCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var id = command.Arguments[1].ToString();
        
        var fields = new Dictionary<string, RedisValue>();

        for (var i = 2; i < command.Arguments.Length; i += 2)
            fields.Add(command.Arguments[i].ToString()!, RedisValue.Create(command.Arguments[i + 1]));

        try
        {
            var result = _storage.XAdd(key!, id!, fields);
            _listener.Signal(key);
            return Task.FromResult<CommandResult>(BulkStringResult.Create(result!));
        }
        catch (RedisException e)
        {
            return Task.FromResult<CommandResult>(ErrorResult.Create(e.Message));
        }
    }
}