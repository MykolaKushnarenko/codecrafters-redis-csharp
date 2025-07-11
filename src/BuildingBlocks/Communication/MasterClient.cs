using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks.Communication;

public class MasterClient : IMasterClient
{
    private readonly Socket _socket;
    private readonly ServerConfiguration _configuration;
    
    public MasterClient(ServerConfiguration configuration)
    {
        _configuration = configuration;
        _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
    }
    
    public async Task<CommunicationResult> SendPing(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        
        var pingCommand = $"*1{Constants.EOL}${Constants.PingCommand.Length}{Constants.EOL}{Constants.PingCommand}{Constants.EOL}";
        await _socket.SendAsync(Encoding.UTF8.GetBytes(pingCommand), cancellationToken);

        var parsedResult = await ReceiveInternalAsync(cancellationToken);

        if (parsedResult.Name.Equals("PONG"))
        {
            return new CommunicationResult { Successed = true };
        }

        return new CommunicationResult() { Successed = false };
    }

    private async Task<ProtocolParseResult?> ReceiveInternalAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];
        await _socket.ReceiveAsync(buffer, cancellationToken);

        using var memoryBuffer = new MemoryStream(buffer);
        var parsedResult = ProtocolParser.Parse(memoryBuffer);
        return parsedResult;
    }

    public async Task<CommunicationResult> SendRepConfigCapa(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        var subCommand = "listening-port";
        var command = "*3\r\n$8\r\nREPLCONF\r\n$4\r\ncapa\r\n$6\r\npsync2\r\n";
        
        await _socket.SendAsync(Encoding.UTF8.GetBytes(command), cancellationToken);

        var parsedResult = await ReceiveInternalAsync(cancellationToken);

        if (parsedResult.Name.Equals("OK"))
        {
            return new CommunicationResult { Successed = true };
        }

        return new CommunicationResult { Successed = false };
    }

    public async Task<CommunicationResult> SendPSync(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);

        var command = "*3\r\n$5\r\nPSYNC\r\n$1\r\n?\r\n$2\r\n-1\r\n";
        
        await _socket.SendAsync(Encoding.UTF8.GetBytes(command), cancellationToken);

        var parsedResult = await ReceiveInternalAsync(cancellationToken);
        
        if (parsedResult.Name.Equals("OK"))
        {
            return new CommunicationResult { Successed = true };
        }

        return new CommunicationResult() { Successed = false };
    }

    public async Task<CommunicationResult> SendRepConfigListeningPort(CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        
        var subCommand = "listening-port";
        var command = $"*3{Constants.EOL}" +
                      $"${Constants.RepconfCommand.Length}{Constants.EOL}{Constants.RepconfCommand}{Constants.EOL}" +
                      $"${subCommand.Length}{Constants.EOL}{subCommand}{Constants.EOL}" +
                      $"${_configuration.Port.ToString().Length}{Constants.EOL}{_configuration.Port}{Constants.EOL}";
        
        await _socket.SendAsync(Encoding.UTF8.GetBytes(command), cancellationToken);

        var parsedResult = await ReceiveInternalAsync(cancellationToken);

        if (parsedResult.Name.Equals("OK"))
        {
            return new CommunicationResult { Successed = true };
        }

        return new CommunicationResult() { Successed = false };
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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}