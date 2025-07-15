using codecrafters_redis.BuildingBlocks.Communication;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Parsers;
using codecrafters_redis.BuildingBlocks.Rdb;
using codecrafters_redis.BuildingBlocks.Storage;

namespace codecrafters_redis.BuildingBlocks;

public class Initiator
{
    private readonly ServerConfiguration _configuration;
    private readonly InMemoryStorage _storage;
    private readonly WatchDog _watchDog;
    private readonly IMasterClient _masterClient;
    private readonly IMediator _mediator;
    private readonly AcknowledgeCommandTracker _tracker;

    public Initiator(
        ServerConfiguration configuration, 
        InMemoryStorage storage, 
        WatchDog watchDog,
        IMasterClient masterClient, 
        IMediator mediator, AcknowledgeCommandTracker tracker)
    {
        _configuration = configuration;
        _storage = storage;
        _watchDog = watchDog;
        _masterClient = masterClient;
        _mediator = mediator;
        _tracker = tracker;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await RehydrateStoreStateAsync(cancellationToken);

        if (_configuration.Role.Equals("slave", StringComparison.CurrentCultureIgnoreCase))
        {
            await SendInitialHandshakeAsync(cancellationToken);
        }
    }

    private async Task SendInitialHandshakeAsync(CancellationToken cancellationToken)
    { 
        await _masterClient.SendPing(cancellationToken);
        await _masterClient.SendRepConfigListeningPort(cancellationToken);
        await _masterClient.SendRepConfigCapa(cancellationToken);
        await _masterClient.SendPSync(cancellationToken);
        await _masterClient.ReceiveRdbFileAsync(cancellationToken);

        _masterClient.Network.Reset();
        //read db.rdb file

        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!_masterClient.IsConnected)
                    {
                        break;
                    }
                    
                    if (!_masterClient.Network.DataAvailable)
                    {
                        continue;
                    }
                    
                    var raspProtocolData = await RaspProtocolParser.ParseCommand(_masterClient.Network, cancellationToken);
                    
                    var result = await _mediator.ProcessAsync(raspProtocolData!, cancellationToken);
                    
                    foreach (var rawResponse in RaspConverter.Convert(result).Where(x => x.Length > 0))
                    {
                        Console.WriteLine("Sending back to master");
                        await _masterClient.Network.WriteAsync(rawResponse, cancellationToken);
                        await _masterClient.Network.FlushAsync(cancellationToken);
                    }
                    
                    _tracker.AddProcessedCommandBytes(_masterClient.Network.ProcessedCommandBytes);
                    _masterClient.Network.Reset();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }, cancellationToken);
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
        
        var dbs = await RdbParser.ParseAsync(file, cancellationToken);
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