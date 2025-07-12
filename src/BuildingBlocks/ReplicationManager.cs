using System.Collections.Concurrent;
using System.Net.Sockets;

namespace codecrafters_redis.BuildingBlocks;

public class ReplicationManager
{
    private readonly ConcurrentBag<Socket> _slaveSockets = new();

    public void AddSlaveForReplication(Socket slave)
    {
        _slaveSockets.Add(slave);
    }

    public async Task ReplicateAsync(byte[] data, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_slaveSockets, cancellationToken, async (slave, token) =>
        {
            await slave.SendAsync(data, token);
        });
    }
}