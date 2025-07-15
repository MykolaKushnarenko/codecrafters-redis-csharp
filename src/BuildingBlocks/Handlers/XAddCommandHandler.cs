using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class XAddCommandHandler : ICommandHandler<Command>
{
    private readonly StreamInMemoryStorage _storage;

    public XAddCommandHandler(StreamInMemoryStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.XAddCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var id = command.Arguments[1].ToString();
        
        _storage.AddValue(key, id, command.Arguments[2].ToString()!, command.Arguments[3].ToString()!);

        return Task.FromResult<CommandResult>(BulkStringResult.Create(id));
    }
}