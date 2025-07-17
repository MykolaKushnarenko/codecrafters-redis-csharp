using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Exceptions;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class XAddCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public XAddCommandHandler(RedisStorage storage)
    {
        _storage = storage;
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
            return Task.FromResult<CommandResult>(BulkStringResult.Create(result!));
        }
        catch (RedisException e)
        {
            return Task.FromResult<CommandResult>(ErrorResult.Create(e.Message));
        }
    }
}