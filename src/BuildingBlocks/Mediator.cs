using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.HandlerFactory;
using Microsoft.Extensions.DependencyInjection;

namespace codecrafters_redis.BuildingBlocks;

public class Mediator : IMediator
{
    private readonly ICommandHandlerFactory _commandHandlerFactory;
    
    public Mediator(ICommandHandlerFactory commandHandlerFactory)
    {
        _commandHandlerFactory = commandHandlerFactory;
    }

    public async Task Process(Context context)
    {
        Console.WriteLine("Processing request");
        while (true)
        {
            byte[] buffer = new byte[1024];

            await context.IncomingSocket.ReceiveAsync(buffer);

            using var memoryStream = new MemoryStream(buffer);
            var result = ProtocolParser.Parse(memoryStream);

            if (result is null)
            {
                break;
            }

            byte[] response;
            
            Console.WriteLine("Getting handler");
            var handler = _commandHandlerFactory.GetHandler(result.Name);
            if (handler != null)
            {
                Console.WriteLine("Executing command");

                var command = new Command { Arguments = result.Arguments };
                response = await handler.HandleAsync(command);
            }
            else
            {
                response = Encoding.UTF8.GetBytes($"-ERR unknown command {result.Name}\r\n");
            }
            
            await context.IncomingSocket.SendAsync(response);
        }
    }
}