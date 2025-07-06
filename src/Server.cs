using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

var storage = new ConcurrentDictionary<string, object>();

const string eom = "\r\n";
while (true)
{
    var socket = server.AcceptSocket();
    
    // fire and forgot
    HandleClientRequest(socket);
}

async Task HandleClientRequest(Socket socket)
{
    while (true)
    {
        var buffer = new byte[1024];
        var received = await socket.ReceiveAsync(buffer, SocketFlags.None);

        var result = ProtocolParser.Parse(buffer);
        
        if (result.Name.Equals("PING", StringComparison.CurrentCultureIgnoreCase))
        {
            var pong = "+PONG\r\n";
            var sendMessageBytes = Encoding.UTF8.GetBytes(pong);
            await socket.SendAsync(sendMessageBytes, SocketFlags.None);
        }

        if (result.Name.Equals("ECHO", StringComparison.CurrentCultureIgnoreCase))
        {
            var echoResponse = result.Arguments[0].ToString();
            var pong = $"${echoResponse.Length}\r\n{echoResponse}\r\n";
            var sendMessageBytes = Encoding.UTF8.GetBytes(pong);
            await socket.SendAsync(sendMessageBytes, SocketFlags.None);
        }
        
        if (result.Name.Equals("SET", StringComparison.CurrentCultureIgnoreCase))
        {
            var inputKey = result.Arguments[0].ToString();
            var inputValue = result.Arguments[1].ToString();
            storage[inputKey] = inputValue;
            var sendMessageBytes = "+OK\r\n"u8.ToArray();
            await socket.SendAsync(sendMessageBytes, SocketFlags.None);
        }
        
        if (result.Name.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
        {
            var inputKey = result.Arguments[0].ToString();
            if(storage.TryGetValue(inputKey, out var value))
            {
                var stringValue = value.ToString();
                var pong = $"${stringValue.Length}\r\n{stringValue}\r\n";
                var sendMessageBytes = Encoding.UTF8.GetBytes(pong);
                await socket.SendAsync(sendMessageBytes, SocketFlags.None);
            }
            else
            {
                var sendMessageBytes = "_\r\n"u8.ToArray();
                await socket.SendAsync(sendMessageBytes, SocketFlags.None);
            }
            
        }
    }
}

