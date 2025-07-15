using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks.Communication;

public class MasterClient : IMasterClient
{
    private readonly Socket _socket;
    private NetworkStream _networkStream;
    private MeasuredNetworkStream _measuredNetworkStream;

    private readonly ServerConfiguration _configuration;
    
    public MasterClient(ServerConfiguration configuration)
    {
        _configuration = configuration;
        _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
    }

    public MeasuredNetworkStream Network => _measuredNetworkStream;

    public bool IsConnected => _socket.Connected;

    public async Task<CommunicationResult> SendPing(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        
        var pingCommand = $"*1{Constants.EOL}${Constants.PingCommand.Length}{Constants.EOL}{Constants.PingCommand}{Constants.EOL}";
        
        await _measuredNetworkStream.WriteAsync(Encoding.UTF8.GetBytes(pingCommand), cancellationToken);
        await _measuredNetworkStream.FlushAsync(cancellationToken);
        
        var raspProtocolData = await ReceiveInternalAsync(cancellationToken);

        if (raspProtocolData.Name.Equals("PONG"))
        {
            return new CommunicationResult { Succeeded = true };
        }
        
        return new CommunicationResult() { Succeeded = false };
    }

    private async Task<RaspProtocolData?> ReceiveInternalAsync(CancellationToken cancellationToken)
    {
        try
        {
            var raspProtocolData = await RaspProtocolParser.ParseCommand(_measuredNetworkStream, cancellationToken);
            return raspProtocolData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<CommunicationResult> SendRepConfigCapa(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        var subCommand = "listening-port";
        var command = "*3\r\n$8\r\nREPLCONF\r\n$4\r\ncapa\r\n$6\r\npsync2\r\n";
        
        await _measuredNetworkStream.WriteAsync(Encoding.UTF8.GetBytes(command), cancellationToken);
        await _measuredNetworkStream.FlushAsync(cancellationToken);

        var raspProtocolData = await ReceiveInternalAsync(cancellationToken);

        if (raspProtocolData.Name.Equals("OK"))
        {
            return new CommunicationResult { Succeeded = true };
        }
        
        return new CommunicationResult { Succeeded = false };
    }

    public async Task<CommunicationResult> SendPSync(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);

        var command = "*3\r\n$5\r\nPSYNC\r\n$1\r\n?\r\n$2\r\n-1\r\n";
        
        await _measuredNetworkStream.WriteAsync(Encoding.UTF8.GetBytes(command), cancellationToken);
        await _measuredNetworkStream.FlushAsync(cancellationToken);
        
        var raspProtocolData = await ReceiveInternalAsync(cancellationToken);
        
        if (raspProtocolData.Name.Equals("OK"))
        {
            return new CommunicationResult { Succeeded = true };
        }

        return new CommunicationResult() { Succeeded = false };
    }

    public async Task<byte[]> ReceiveRdbFileAsync(CancellationToken cancellationToken)
    { 
        try
        {
            var receiveRdbFile = await RaspProtocolParser.ParseBinaryAsync(_measuredNetworkStream);
            return receiveRdbFile;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return Enumerable.Empty<byte>().ToArray();
    }

    public async Task<CommunicationResult> SendRepConfigListeningPort(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        
        var subCommand = "listening-port";
        var command = $"*3{Constants.EOL}" +
                      $"${Constants.RepconfCommand.Length}{Constants.EOL}{Constants.RepconfCommand}{Constants.EOL}" +
                      $"${subCommand.Length}{Constants.EOL}{subCommand}{Constants.EOL}" +
                      $"${_configuration.Port.ToString().Length}{Constants.EOL}{_configuration.Port}{Constants.EOL}";
        
        await _measuredNetworkStream.WriteAsync(Encoding.UTF8.GetBytes(command), cancellationToken);
        await _measuredNetworkStream.FlushAsync(cancellationToken);
        
        var raspProtocolData = await ReceiveInternalAsync(cancellationToken);

        if (raspProtocolData.Name.Equals("OK"))
        {
            return new CommunicationResult { Succeeded = true };
        }
        
        return new CommunicationResult() { Succeeded = false };
    }

    private async Task TryConnectAsync(CancellationToken cancellationToken)
    {
        if (_socket.Connected)
        {
            return;
        }

        var host = await Dns.GetHostEntryAsync(_configuration.MasterHost, cancellationToken);
        var ipEndPoint = new IPEndPoint(host.AddressList[0], _configuration.MasterPort);

        try
        {
            await _socket.ConnectAsync(ipEndPoint, cancellationToken);
            _networkStream = new NetworkStream(_socket, FileAccess.ReadWrite);
            _measuredNetworkStream = new MeasuredNetworkStream(_networkStream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}