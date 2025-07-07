using System.Text;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class GetCommandHandler : ICommandHandler<GetCommand>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    
    public GetCommandHandler(InMemoryStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }
    
    public Task<byte[]> HandleAsync(GetCommand command)
    {
        var key = command.Arguments[0].ToString(); 
        
        if (_watchDog.IsExpired(key!))
        {
            _storage.Remove(key!);
            return Task.FromResult(Encoding.UTF8.GetBytes(Constants.BulkStringEmptyResponse));
        }
        
        var value = _storage.Get(key!);

        return Task.FromResult(
            Encoding.UTF8.GetBytes($"${value.ToString()!.Length}{Constants.EOL}{value}{Constants.EOL}"));
    }
}