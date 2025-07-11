using System.Net;
using System.Net.Sockets;
using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Configurations;
using codecrafters_redis.BuildingBlocks.Extensions;
using Microsoft.Extensions.DependencyInjection;

// You can use print statements as follows for debugging, they'll be visible when running tests.

var serviceCollection = new ServiceCollection();
var provider = serviceCollection.AddBuildingBlocks(args).BuildServiceProvider();

var initeiator = provider.GetRequiredService<Initiator>();
await initeiator.InitializeAsync();

var configuration = provider.GetRequiredService<ServerConfiguration>();

// Uncomment this block to pass the first stage
var server = new TcpListener(IPAddress.Any, configuration.Port);
server.Start();

while (true)
{
    var socket = server.AcceptSocket();
    
    // fire and forgot
    _ = HandleClientRequest(socket).ContinueWith((t) =>
    {
        Console.WriteLine("Client disconnected");
        socket.Close();
    });
}

async Task HandleClientRequest(Socket socket)
{
    await provider.GetRequiredService<IMediator>().Process(new Context { IncomingSocket = socket });
}

