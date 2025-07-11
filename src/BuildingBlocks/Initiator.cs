using codecrafters_redis.BuildingBlocks.Communication;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.DB;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks;

public class Initiator
{
    private readonly ServerConfiguration _configuration;
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    private readonly IMasterClient _masterClient;
    
    public Initiator(ServerConfiguration configuration, InMemoryStorage storage, WatchDog watchDog, IMasterClient masterClient)
    {
        _configuration = configuration;
        _storage = storage;
        _watchDog = watchDog;
        _masterClient = masterClient;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await RehydrateStoreStateAsync(cancellationToken);

        if (_configuration.Role.Equals("slave", StringComparison.CurrentCultureIgnoreCase))
        {
            var result = await _masterClient.SendPing(cancellationToken);
            await _masterClient.SendRepConfigListeningPort(cancellationToken);
            await _masterClient.SendRepConfigCapa(cancellationToken);
            await _masterClient.SendPSync(cancellationToken);
        }
    }

    private async Task RehydrateStoreStateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_configuration.Dir) || string.IsNullOrEmpty(_configuration.DbFileName))
        {
            return;
        }
        
        var filePath = $"{_configuration.Dir}/{_configuration.DbFileName}";
        var file = File.Open(filePath, FileMode.OpenOrCreate);

        if (file.Length == 0)
        {
            return;
        }
        
        var dbs = await DbParser.ParseAsync(file, cancellationToken);
        var firstDb = dbs.First();

        foreach (var keyValue in firstDb.KeyValues)
        {
            _storage.Set(keyValue.Key, keyValue.Value);
        }

        foreach (var keyExpirationTimestamp in firstDb.KeyExpirationTimestamps)
        {
            _watchDog.Watch(keyExpirationTimestamp.Key, keyExpirationTimestamp.Value);
        }
    }
}