using System.Net.Sockets;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Parsers;

namespace DotRedis.BuildingBlocks.Services;

// subscribe once
// check if we can perform other rommands
// push message to the subscribers subscribed to a channel

public class SubscriptionManager
{
    private readonly Dictionary<string, List<Socket>> _subscribers = new();
    
    private readonly AsyncLocal<ConnectionSubscriptionMetadata> _connectionSubscriptionMetadata = new ();
    
    private static readonly SemaphoreSlim Gate = new(1);

    public void Initiate()
    {
        _connectionSubscriptionMetadata.Value = new ConnectionSubscriptionMetadata();
    }
    
    public async Task SubscribeToChannelAsync(string channel, Socket subscriber)
    {
        await Gate.WaitAsync();
        try
        {
            var sockets = _subscribers.GetValueOrDefault(channel, []);
            sockets.Add(subscriber);
            _subscribers[channel] = sockets;
            
            _connectionSubscriptionMetadata.Value.HasAnySubscription = true;
            _connectionSubscriptionMetadata.Value.ChannelSubscribedToCount++;
        }
        finally
        {
            Gate.Release();
        }
    }
    
    public async ValueTask<int> PublishMessageAsync(string channel, CommandResult commandResult, CancellationToken cancellationToken)
    {
        //insdead of blocking a client we need to queue messages to user
        //add error handling

        await Gate.WaitAsync();

        try
        {
            var subscribers = _subscribers.GetValueOrDefault(channel, []);
            if (subscribers.Count == 0)
            {
                Console.WriteLine("Not subscribed to a channel");
                return 0;
            }

            var responses = RaspConverter.Convert(commandResult).Where(x => x.Length > 0);

            foreach (var subscriber in subscribers)
            {
                foreach (var response in responses)
                {
                    await subscriber.SendAsync(response, cancellationToken);
                }
            }
        }
        finally
        {
            Gate.Release();
        }

        return 0;
    }
    
    public int SubscriberCount => _connectionSubscriptionMetadata.Value.ChannelSubscribedToCount;
    
    public bool HasAnySubscription => _connectionSubscriptionMetadata.Value.HasAnySubscription;
    
    private class ConnectionSubscriptionMetadata
    {
        public int ChannelSubscribedToCount { get; set; }
        public bool HasAnySubscription { get; set; }
    }
}