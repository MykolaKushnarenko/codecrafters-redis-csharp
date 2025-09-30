using System.Net.Sockets;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Parsers;

namespace DotRedis.BuildingBlocks.Services;

// subscribe once
// check if we can perform other rommands
// push message to the subscribers subscribed to a channel

public class SubscriptionManager
{
    private readonly AsyncLocal<Subscription> _subscriber = new AsyncLocal<Subscription>();

    public void Initiate(Socket subscriber)
    {
        _subscriber.Value = new Subscription
        {
            Channel = [],
            Subscriber = subscriber
        };
    }
    
    public void SubscribeToChannel(string channel)
    {
        if(_subscriber.Value.Channel.Contains(channel))
            return;
        
        _subscriber.Value.Channel.Add(channel);
    }
    
    public async ValueTask PublishMessageAsync(string channel, CommandResult commandResult, CancellationToken cancellationToken)
    {
        //insdead of blocking a client we need to queue messages to user
        //add error handling

        if (!_subscriber.Value.Channel.Contains(channel))
        {
            Console.WriteLine("Not subscribed to a channel");
            return;
        }
        
        var responses = RaspConverter.Convert(commandResult).Where(x => x.Length > 0);

        foreach (var response in responses)
        {
            await _subscriber.Value.Subscriber.SendAsync(response, cancellationToken);
        }
    }
    
    public bool IsSubscribedToAnyChannel => _subscriber.Value.Channel.Count > 0;
    
    public int SubscriberCount => _subscriber.Value.Channel.Count;
    
    private class Subscription
    {
        public List<string> Channel { get; set; }
        public Socket Subscriber { get; set; }
    }
}