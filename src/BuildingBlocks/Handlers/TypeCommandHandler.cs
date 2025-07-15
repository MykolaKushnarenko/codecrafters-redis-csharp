using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class TypeCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;

    public TypeCommandHandler(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.TypeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();

        var value = _storage.Get(key);

        if (value is null)
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create("none"));
        }
        
        return Task.FromResult<CommandResult>(SimpleStringResult.Create("string"));
    }
}