using System.Net;
using System.Net.Sockets;
using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks;

public class Server
{
    private readonly IMediator _mediator;
    private readonly ServerConfiguration _configuration;
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly Initiator _initiator;

    public Server(
        IMediator mediator, 
        ServerConfiguration configuration, 
        ApplicationLifetime applicationLifetime,
        Initiator initiator)
    {
        _mediator = mediator;
        _configuration = configuration;
        _applicationLifetime = applicationLifetime;
        _initiator = initiator;
    }

    public async Task RunAsync()
    {
        var cancellation = _applicationLifetime.CreateApplicationCancellationToken();
        
        await _initiator.InitializeAsync(cancellation);
        
        var listener = new TcpListener(IPAddress.Any, _configuration.Port);
        listener.Start();

        while (!cancellation.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync(cancellation);
    
            // fire and forgot
            _ = HandleClientRequest(socket, cancellation)
                .ContinueWith(_ =>
            {
                socket.Close();
            });
        }
    }
    
    private async Task HandleClientRequest(Socket socket, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.ProcessAsync(new Context { IncomingSocket = socket }, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}