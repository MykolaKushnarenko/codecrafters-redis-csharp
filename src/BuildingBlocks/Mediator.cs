using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace codecrafters_redis.BuildingBlocks;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Process(Context context)
    {
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
            switch (result.Name.ToUpperInvariant())
            {
                case "PING":
                {
                    var handler = _serviceProvider.GetRequiredService<PingCommandHandler>();
                    response = await handler.HandleAsync(new PingCommand { Arguments = result.Arguments });
                    break;
                }
                case "ECHO":
                {
                    var handler = _serviceProvider.GetRequiredService<EchoCommandHandler>();
                    response = await handler.HandleAsync(new EchoCommand { Arguments = result.Arguments });
                    break;
                }
                case "SET":
                {
                    var handler = _serviceProvider.GetRequiredService<SetCommandHandler>();
                    response = await handler.HandleAsync(new SetCommand { Arguments = result.Arguments });
                    break;
                }
                case "GET":
                {
                    var handler = _serviceProvider.GetRequiredService<GetCommandHandler>();
                    response = await handler.HandleAsync(new GetCommand { Arguments = result.Arguments });
                    break;
                }
                case "CONFIG":
                {
                    var handler = _serviceProvider.GetRequiredService<ConfigCommandHandler>();
                    response = await handler.HandleAsync(new ConfigCommand { Arguments = result.Arguments });
                    break;
                }
                default:
                    response = Encoding.UTF8.GetBytes($"-ERR unknown command {result.Name}\r\n");
                    break;
            }

            await context.IncomingSocket.SendAsync(response);
        }
    }
}