using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.HandlerFactory;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public class Mediator : IMediator
{
    private readonly ICommandHandlerFactory _commandHandlerFactory;
    
    public Mediator(ICommandHandlerFactory commandHandlerFactory)
    {
        _commandHandlerFactory = commandHandlerFactory;
    }

    public async Task ProcessAsync(Context context, CancellationToken cancellationToken)
    {
        while (true)
        {
            byte[] buffer = new byte[1024];

            await context.IncomingSocket.ReceiveAsync(buffer, cancellationToken);

            using var memoryStream = new MemoryStream(buffer);
            var result = ProtocolParser.Parse(memoryStream);

            if (result is null)
            {
                break;
            }

            await ProcessIncomingCommandAsync(context, cancellationToken, result);
        }
    }

    private async Task ProcessIncomingCommandAsync(Context context, CancellationToken cancellationToken,
        ProtocolParseResult result)
    {
        var handler = _commandHandlerFactory.GetHandler(result.Name);
        if (handler != null)
        {
            var command = new Command { Arguments = result.Arguments };
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