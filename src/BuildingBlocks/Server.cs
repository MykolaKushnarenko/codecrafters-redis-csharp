using System.Net;
using System.Net.Sockets;
using DotRedis.BuildingBlocks.Configurations;
using DotRedis.BuildingBlocks.Parsers;
using DotRedis.BuildingBlocks.Services;

namespace DotRedis.BuildingBlocks;

/// <summary>
///     Represents a server that handles client requests asynchronously.
///     Initializes necessary services and manages the lifecycle of client connections.
/// </summary>
public class Server
{
    private readonly IMediator _mediator;
    private readonly ServerConfiguration _configuration;
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly Initiator _initiator;
    private readonly ReplicationManager _replicationManager;
    private readonly AcknowledgeCommandTracker _acknowledge;
    private readonly TransactionManager _transactionManager;
    private readonly SocketAccessor _socketAccessor;

    public Server(
        IMediator mediator, 
        ServerConfiguration configuration, 
        ApplicationLifetime applicationLifetime,
        Initiator initiator, 
        ReplicationManager replicationManager, 
        AcknowledgeCommandTracker acknowledge, 
        TransactionManager transactionManager, 
        SocketAccessor socketAccessor)
    {
        _mediator = mediator;
        _configuration = configuration;
        _applicationLifetime = applicationLifetime;
        _initiator = initiator;
        _replicationManager = replicationManager;
        _acknowledge = acknowledge;
        _transactionManager = transactionManager;
        _socketAccessor = socketAccessor;
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
    
            Console.WriteLine($"Accepted connection from {socket.RemoteEndPoint}");
            
            // fire and forgot
            _ = HandleClientRequest(socket, cancellation)
                .ContinueWith(_ =>
            {
                socket.Close();
            }, cancellation);
        }
    }
    
    private async Task HandleClientRequest(Socket socket, CancellationToken cancellationToken)
    {
        try
        {
            _transactionManager.Initiate();
            _socketAccessor.SetSocket(socket);
            
            await using var networkStream = new NetworkStream(socket, FileAccess.ReadWrite);
            await using var measuredNetworkStream = new MeasuredNetworkStream(networkStream);
            while (!cancellationToken.IsCancellationRequested || !socket.Connected)
            {
                var raspProtocolData = await RaspProtocolParser.ParseCommand(measuredNetworkStream, cancellationToken);
                
                if (raspProtocolData.Name == Constants.PsyncCommand && _configuration.Role == "master")
                {
                    _replicationManager.AddSlaveForReplication(networkStream);
                }

                raspProtocolData.CommandByteLength = measuredNetworkStream.ProcessedCommandBytes;
                
                var result = await _mediator.ProcessAsync(raspProtocolData, cancellationToken);
            
                foreach (var rawResponse in RaspConverter.Convert(result).Where(x => x.Length > 0))
                {
                    await measuredNetworkStream.WriteAsync(rawResponse, cancellationToken);
                    await measuredNetworkStream.FlushAsync(cancellationToken);
                }
                
                _acknowledge.AddProcessedCommandBytes(measuredNetworkStream.ProcessedCommandBytes);
                measuredNetworkStream.Reset();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}