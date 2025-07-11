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

    public async Task InitializeAsync()
    {
        await RehydrateStoreStateAsync();

        if (_configuration.Role.Equals("slave", StringComparison.CurrentCultureIgnoreCase))
        {
            Console.WriteLine("Sending master a request");
            var result = await _masterClient.Ping();
            Console.WriteLine($"Response {result.Successed}");
        }
    }

    private async Task RehydrateStoreStateAsync()
    {
        if (string.IsNullOrEmpty(_configuration.Dir) || string.IsNullOrEmpty(_configuration.DbFileName))
        {
            return;
        }
        
        var filePath = $"{_configuration.Dir}/{_configuration.DbFileName}";
        Console.WriteLine(filePath);
        var file = File.Open(filePath, FileMode.OpenOrCreate);

        if (file.Length == 0)
        {
            return;
        }
        
        var dbs = await DbParser.Parse(file);
        var firstDb = dbs.First();

        foreach (var keyValue in firstDb.KeyValues)
        {
            Console.WriteLine($"Inserting {keyValue.Key}");
            _storage.Set(keyValue.Key, keyValue.Value);
        }

        foreach (var keyExpirationTimestamp in firstDb.KeyExpirationTimestamps)
        {
            _watchDog.Watch(keyExpirationTimestamp.Key, keyExpirationTimestamp.Value);
        }
    }
}