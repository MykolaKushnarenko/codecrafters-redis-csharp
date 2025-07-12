using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.HandlerFactory;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public class Mediator : IMediator
{
    private readonly ICommandHandlerFactory _commandHandlerFactory;
    private readonly ReplicationManager _replicationManager;
    
    public Mediator(ICommandHandlerFactory commandHandlerFactory, ReplicationManager replicationManager)
    {
        _commandHandlerFactory = commandHandlerFactory;
        _replicationManager = replicationManager;
    }

    public async Task ProcessAsync(Context context, CancellationToken cancellationToken)
    {
        while (true)
        {
            var buffer = new byte[1024];

            var receivedBytes = await context.IncomingSocket.ReceiveAsync(buffer, cancellationToken);

            using var memoryStream = new MemoryStream(buffer[..receivedBytes]);
            var result = ProtocolParser.Parse(memoryStream);

            if (result is null)
            {
                break;
            }

            await ProcessIncomingCommandAsync(context, buffer[..receivedBytes], cancellationToken, result);
        }
    }

    private async Task ProcessIncomingCommandAsync(Context context, byte[] initialCommandBytes, CancellationToken cancellationToken,
        ProtocolParseResult result)
    {
        if (result.Name == Constants.PsyncCommand)
        {
            _replicationManager.AddSlaveForReplication(context.IncomingSocket);
        }
        
        var handler = _commandHandlerFactory.GetHandler(result.Name);
        if (handler != null)
        {
            var command = new Command { InitialCommandBytes = initialCommandBytes, Arguments = result.Arguments };
            var commandResult = await handler.HandleAsync(command, cancellationToken);
            foreach (var rawResponse in CommandResultConverter.Convert(commandResult))
            {
                await context.IncomingSocket.SendAsync(rawResponse, cancellationToken);
            }
        }
        else
        {
            await context.IncomingSocket.SendAsync(Encoding.UTF8.GetBytes($"-ERR unknown command {result.Name}\r\n"),
                cancellationToken);
        }
    }
}