using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public class Server
{
    private readonly IMediator _mediator;
    private readonly ServerConfiguration _configuration;
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly Initiator _initiator;
    private readonly ReplicationManager _replicationManager;

    public Server(
        IMediator mediator, 
        ServerConfiguration configuration, 
        ApplicationLifetime applicationLifetime,
        Initiator initiator, 
        ReplicationManager replicationManager)
    {
        _mediator = mediator;
        _configuration = configuration;
        _applicationLifetime = applicationLifetime;
        _initiator = initiator;
        _replicationManager = replicationManager;
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
            var networkStream = new NetworkStream(socket, FileAccess.ReadWrite);
            while (!cancellationToken.IsCancellationRequested)
            {
                var raspProtocolData = await RaspProtocolParser.ParseCommand(networkStream);
                
                if (raspProtocolData.Name == Constants.PsyncCommand && _configuration.Role == "master")
                {
                    _replicationManager.AddSlaveForReplication(networkStream);
                }
                
                if (raspProtocolData is null)
                {
                    continue;
                }
            
                var result = await _mediator.ProcessAsync(raspProtocolData, cancellationToken);
                
                if (result is MasterReplicationResult)
                {
                    return;
                }
            
                foreach (var rawResponse in RaspConverter.Convert(result))
                {
                    Console.WriteLine("Sending back to master");
                    await networkStream.WriteAsync(rawResponse, cancellationToken);
                    await networkStream.FlushAsync(cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}