using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;
using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Handlers;
using codecrafters_redis.BuildingBlocks.Storage;
using Microsoft.Extensions.DependencyInjection;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<InMemoryStorage>();
serviceCollection.AddSingleton<WatchDog>();
serviceCollection.AddSingleton<IMediator, Mediator>();
serviceCollection.AddSingleton<PingCommandHandler>();
serviceCollection.AddSingleton<EchoCommandHandler>();
serviceCollection.AddSingleton<SetCommandHandler>();
serviceCollection.AddSingleton<GetCommandHandler>();

var provider = serviceCollection.BuildServiceProvider();

// Uncomment this block to pass the first stage
var server = new TcpListener(IPAddress.Any, 6379);
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

