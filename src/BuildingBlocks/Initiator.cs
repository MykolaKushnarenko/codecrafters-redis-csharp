using codecrafters_redis.BuildingBlocks.Storage;
using DotRedis.BuildingBlocks.Communication;
using DotRedis.BuildingBlocks.Configurations;
using DotRedis.BuildingBlocks.Parsers;
using DotRedis.BuildingBlocks.Rdb;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks;

/// <summary>
///     The <c>Initiator</c> class is responsible for initializing necessary components and performing setup actions.
///     It handles tasks like restoring the store state and sending an initial handshake based on server role.
/// </summary>
public class Initiator
{
    private readonly ServerConfiguration _configuration;
    private readonly RedisStorage _storage;
    private readonly WatchDog _watchDog;
    private readonly IMasterClient _masterClient;
    private readonly IMediator _mediator;
    private readonly AcknowledgeCommandTracker _tracker;
    private readonly TransactionManager _manager;
    private readonly SubscriptionManager _subscriptionManager;

    public Initiator(
        ServerConfiguration configuration, 
        RedisStorage storage, 
        WatchDog watchDog,
        IMasterClient masterClient, 
        IMediator mediator, 
        AcknowledgeCommandTracker tracker, 
        TransactionManager manager, 
        SubscriptionManager subscriptionManager)
    {
        _configuration = configuration;
        _storage = storage;
        _watchDog = watchDog;
        _masterClient = masterClient;
        _mediator = mediator;
        _tracker = tracker;
        _manager = manager;
        _subscriptionManager = subscriptionManager;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await RehydrateStoreStateAsync(cancellationToken);

        if (_configuration.Role.Equals("slave", StringComparison.CurrentCultureIgnoreCase))
        {
            await SendInitialHandshakeAsync(cancellationToken);
        }
    }

    // TODO: Move this to a separate class
    // Hadnshake is the responsibility of a slave.
    private async Task SendInitialHandshakeAsync(CancellationToken cancellationToken)
    { 
        await _masterClient.SendPing(cancellationToken);
        await _masterClient.SendRepConfigListeningPort(cancellationToken);
        await _masterClient.SendRepConfigCapa(cancellationToken);
        await _masterClient.SendPSync(cancellationToken);
        
        //TODO: We receive the RDB file but did not use it.We need to rehydrate the store state from the RDB file.
        await _masterClient.ReceiveRdbFileAsync(cancellationToken);

        _masterClient.Network.Reset();
        
        //TODO: Duplicate code. We need to refactor this.
        _ = Task.Run(async () =>
        {
            _manager.Initiate();
            
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
            _storage.Set(keyValue.Key, RedisValue.Create(keyValue.Value));
        }

        foreach (var keyExpirationTimestamp in firstDb.KeyExpirationTimestamps)
        {
            _watchDog.Watch(keyExpirationTimestamp.Key, keyExpirationTimestamp.Value);
        }
    }
}