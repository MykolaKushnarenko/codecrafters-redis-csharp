using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class SubscribeCommandHandler : ICommandHandler<Command>
{
    private readonly SubscriptionManager _subscriptionManager;

    public SubscribeCommandHandler(
        SubscriptionManager subscriptionManager)
    {
        _subscriptionManager = subscriptionManager;
    }

    public string HandlingCommandName => Constants.SubscribeCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var channels = command.Arguments.Select(arg => arg.ToString()).ToArray();

        var channelNamesResponse = new List<CommandResult>();
        foreach (var channel in channels)
        {
            _subscriptionManager.SubscribeToChannel(channel);
            channelNamesResponse.Add(BulkStringResult.Create(channel));
        }
        
        var result = ArrayResult.Create(BulkStringResult.Create("subscribe"));
        result.Add(channelNamesResponse.ToArray());
        result.Add(IntegerResult.Create(_subscriptionManager.SubscriberCount));
        
        return Task.FromResult<CommandResult>(result);
    }
}