using System.Text;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

// *5\r\n$3\r\nSET\r\n$9\r\nraspberry\r\n$5\r\ngrape\r\n$2\r\npx\r\n$3\r\n100\r\n

public class SetCommandHandler : ICommandHandler<SetCommand>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;

    public SetCommandHandler(InMemoryStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }

    public Task<byte[]> HandleAsync(SetCommand command)
    {
        var key = command.Arguments[0].ToString();
        var value = command.Arguments[1].ToString();

        if (command.Arguments.Length > 2 && command.Arguments[2].ToString()!.Equals("PX", StringComparison.InvariantCultureIgnoreCase))
        {
            var expiration = int.Parse(command.Arguments[3].ToString()!);
            _watchDog.Watch(key!, DateTimeOffset.UtcNow.AddMilliseconds(expiration));
        }
        
        _storage.Set(key!, value!);

        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));
    }
}