using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class UnsubscribeCommandHandler : ICommandHandler<Command>
{
    private readonly SubscriptionManager _subscriptionManager;
    private readonly SocketAccessor _socketAccessor;

    public UnsubscribeCommandHandler(SocketAccessor socketAccessor, SubscriptionManager subscriptionManager)
    {
        _socketAccessor = socketAccessor;
        _subscriptionManager = subscriptionManager;
    }

    public string HandlingCommandName => Constants.UnsubscribeCommand;
    
    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var channel = command.Arguments[0].ToString(); 
        var currentConnection = _socketAccessor.GetSocket();
        await _subscriptionManager.UnsubscribeAsync(channel, currentConnection, cancellationToken);

        var arrayResult = ArrayResult.Create(BulkStringResult.Create("unsubscribe"));
        arrayResult.Add(BulkStringResult.Create(channel));
        arrayResult.Add(IntegerResult.Create(_subscriptionManager.SubscriptionCount));

        return arrayResult;
    }
}