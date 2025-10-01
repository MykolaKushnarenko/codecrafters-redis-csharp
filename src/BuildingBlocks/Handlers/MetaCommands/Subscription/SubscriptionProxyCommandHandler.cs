using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.MetaCommands.Subscription;

public class SubscriptionProxyCommandHandler : ICommandHandler<Command>
{
    private static readonly List<string> _allowedCommands =
    [
        Constants.SubscribeCommand, 
        Constants.PingCommand,
        //Constants.UnsubscribeCommand, 
        //Constants.PSubscribeCommand,
        //Constants.PUnsubscribeCommand,
        //Constants.QuitCommand
    ];

    private readonly ICommandHandler<Command> _innerHandler;
    private readonly SubscriptionManager _subscriptionManager;
    
    public SubscriptionProxyCommandHandler(
        ICommandHandler<Command> innerHandler, 
        SubscriptionManager subscriptionManager)
    {
        _innerHandler = innerHandler;
        _subscriptionManager = subscriptionManager;
    }

    public string HandlingCommandName => _innerHandler.HandlingCommandName;

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        if (!_subscriptionManager.HasAnySubscription || _allowedCommands.Contains(_innerHandler.HandlingCommandName))
        {
            return _innerHandler.HandleAsync(command, cancellationToken);
        }

        return Task.FromResult<CommandResult>(ErrorResult.Create($"Can't execute '{_innerHandler.HandlingCommandName}': only (P|S)SUBSCRIBE / (P|S)UNSUBSCRIBE / PING / QUIT / RESET are allowed in this context"));
    }
}