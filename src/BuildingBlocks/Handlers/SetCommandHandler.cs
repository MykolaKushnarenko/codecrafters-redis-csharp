using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class SetCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;

    public SetCommandHandler(InMemoryStorage storage, WatchDog watchDog)
    {
        _storage = storage;
        _watchDog = watchDog;
    }
    
    public string HandlingCommandName => Constants.SetCommand;

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var value = command.Arguments[1].ToString();

        if (command.Arguments.Length > 2 && command.Arguments[2].ToString()!.Equals("PX", StringComparison.InvariantCultureIgnoreCase))
        {
            var expiration = int.Parse(command.Arguments[3].ToString()!);
            _watchDog.Watch(key!, DateTimeOffset.UtcNow.AddMilliseconds(expiration));
        }
        
        _storage.Set(key!, value!);
        
        return Task.FromResult<CommandResult>(SimpleStringResult.Create(Constants.OkResponse));
    }
}