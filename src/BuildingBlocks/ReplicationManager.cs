using System.Collections.Concurrent;
using System.Net.Sockets;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public class ReplicationManager
{
    private readonly ConcurrentBag<NetworkStream> _slaveSockets = new();

    public void AddSlaveForReplication(NetworkStream slave)
    {
        _slaveSockets.Add(slave);
    }

    public async Task ReplicateAsync(CommandResult command, CancellationToken cancellationToken)
    {
        var data = RaspConverter.Convert(command).First();
        
        await Parallel.ForEachAsync(_slaveSockets, cancellationToken, async (slave, token) =>
        {
            await slave.WriteAsync(data, token);
            await slave.FlushAsync(token);
        });
    }
}