using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class SetCommandHandler : ICommandHandler<Command>
{
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    private readonly ReplicationManager _replicationManager;
    private readonly ServerConfiguration _configuration;

    public SetCommandHandler(InMemoryStorage storage, WatchDog watchDog, ReplicationManager replicationManager, ServerConfiguration configuration)
    {
        _storage = storage;
        _watchDog = watchDog;
        _replicationManager = replicationManager;
        _configuration = configuration;
    }
    
    public string HandlingCommandName => Constants.SetCommand;

    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        try
        {
            var key = command.Arguments[0].ToString();
            var value = command.Arguments[1].ToString();

            if (command.Arguments.Length > 2 && command.Arguments[2].ToString()!.Equals("PX", StringComparison.InvariantCultureIgnoreCase))
            {
                var expiration = int.Parse(command.Arguments[3].ToString()!);
                _watchDog.Watch(key!, DateTimeOffset.UtcNow.AddMilliseconds(expiration));
            }
        
            _storage.Set(key!, value!);
            if (_configuration.Role == "master")
            {
                var list = new List<CommandResult>();
                list.Add(BulkStringResult.Create(HandlingCommandName));
                foreach (var argument in command.Arguments)
                {
                    list.Add(BulkStringResult.Create(argument.ToString()!));
                }
                await _replicationManager.ReplicateAsync(ArrayResult.Create(list.ToArray()), cancellationToken);
                return SimpleStringResult.Create(Constants.OkResponse);
            }

            return new MasterReplicationResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}