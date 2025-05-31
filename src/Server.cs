using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var socket = server.AcceptSocket(); // wait for client

const string eom = "\r\n";
while (true)
{
    var buffer = new byte[1024];
    var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
    
    var response = Encoding.UTF8.GetString(buffer, 0, received);

    if (response.IndexOf(eom) > -1 /* is end of message */)
    {
        await socket.SendAsync("+PONG\r\n"u8.ToArray());
    }
}

