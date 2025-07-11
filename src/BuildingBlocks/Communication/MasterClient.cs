using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis.BuildingBlocks.Configurations;

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
    
    public async Task<CommunicationResult> Ping()
    {
        await TryConnectAsync();
        
        var pingCommand = $"*1{Constants.EOL}${Constants.PingCommand.Length}{Constants.EOL}{Constants.PingCommand}{Constants.EOL}";
        await _socket.SendAsync(Encoding.UTF8.GetBytes(pingCommand));

        var buffer = new byte[1024];
        await _socket.ReceiveAsync(buffer);

        Console.WriteLine("Received response");
        using var memoryBuffer = new MemoryStream(buffer);
        var parsedResult = ProtocolParser.Parse(memoryBuffer);

        Console.WriteLine($"Reply received {parsedResult.Name}");
        if (parsedResult.Name.Equals("PONG"))
        {
            return new CommunicationResult { Successed = true };
        }

        return new CommunicationResult() { Successed = false };
    }

    private async Task TryConnectAsync()
    {
        if (_socket.Connected)
        {
            return;
        }

        var host = await Dns.GetHostEntryAsync(_configuration.MasterHost);
        var ipEndPoint = new IPEndPoint(host.AddressList[0], _configuration.MasterPort);

        try
        {
            await _socket.ConnectAsync(ipEndPoint);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}