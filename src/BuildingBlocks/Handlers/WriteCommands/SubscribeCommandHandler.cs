using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class SubscribeCommandHandler : ICommandHandler<Command>
{
    private readonly SubscriptionManager _subscriptionManager;
    private readonly SocketAccessor _socketAccessor;

    public SubscribeCommandHandler(
        SubscriptionManager subscriptionManager, 
        SocketAccessor socketAccessor)
    {
        _subscriptionManager = subscriptionManager;
        _socketAccessor = socketAccessor;
    }

    public string HandlingCommandName => Constants.SubscribeCommand;
    
    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var channels = command.Arguments.Select(arg => arg.ToString()).ToArray();

        var channelNamesResponse = new List<CommandResult>();
        foreach (var channel in channels)
        {
            await _subscriptionManager.SubscribeToChannelAsync(channel, _socketAccessor.GetSocket());
            channelNamesResponse.Add(BulkStringResult.Create(channel));
        }
        
        var result = ArrayResult.Create(BulkStringResult.Create("subscribe"));
        result.Add(channelNamesResponse.ToArray());
        result.Add(IntegerResult.Create(_subscriptionManager.SubscriptionCount));
        
        return result;
    }
}