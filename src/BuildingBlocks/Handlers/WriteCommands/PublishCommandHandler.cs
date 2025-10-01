using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class PublishCommandHandler : ICommandHandler<Command>
{
    private readonly SubscriptionManager _subscriptionManager;
    
    public PublishCommandHandler(SubscriptionManager subscriptionManager)
    {
        _subscriptionManager = subscriptionManager;
    }
    
    public string HandlingCommandName => Constants.PublishCommand;

    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var channel = command.Arguments[0].ToString();
        var message = command.Arguments[1].ToString();

        var count = await _subscriptionManager.PublishMessageAsync(channel, message, cancellationToken);
        
        return IntegerResult.Create(count);
    }
}