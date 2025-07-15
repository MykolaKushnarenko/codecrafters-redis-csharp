using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class TypeCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;
    private readonly StreamInMemoryStorage _steamInMemoryStorage;

    public TypeCommandHandler(InMemoryStorage storage, StreamInMemoryStorage steamInMemoryStorage)
    {
        _storage = storage;
        _steamInMemoryStorage = steamInMemoryStorage;
    }

    public string HandlingCommandName => Constants.TypeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();

        var value = _storage.Get(key);
        var streamValue = _steamInMemoryStorage.Get(key);

        if (value is null && streamValue is null)
        {
            return Task.FromResult<CommandResult>(SimpleStringResult.Create("none"));
        }
        
        var type = value is null
            ? StorageItemTypeMapper.GetStorageType(streamValue.GetType()).ToString()
                : StorageItemTypeMapper.GetStorageType(value.GetType()).ToString();
        
        return Task.FromResult<CommandResult>(
            SimpleStringResult.Create(type.ToLowerInvariant()));
    }
}