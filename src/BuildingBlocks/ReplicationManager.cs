using System.Collections.Concurrent;
using System.Net.Sockets;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public class ReplicationManager
{
    private readonly ConcurrentBag<NetworkStream> _slaveSockets = new();
    
    private int _syncedReplicasCount = 0;
    private long _writeCommandOffset = 0;

    public int NumberOfReplicas => _slaveSockets.Count;

    public int SyncedReplicasCount => _syncedReplicasCount;

    public long WriteCommandOffset => _writeCommandOffset;
    
    public void AddSlaveForReplication(NetworkStream slave)
    {
        _slaveSockets.Add(slave);
    }

    public async Task ReplicateAsync(CommandResult command, CancellationToken cancellationToken)
    {
        Interlocked.Exchange(ref _syncedReplicasCount, 4);
        
        var data = RaspConverter.Convert(command).First();
        
        await Parallel.ForEachAsync(_slaveSockets, cancellationToken, async (slave, token) =>
        {
            try
            {
                await slave.WriteAsync(data, token);
                await slave.FlushAsync(token);

                Interlocked.Increment(ref _syncedReplicasCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    public async Task GetAcksAsync(CancellationToken cancellationToken)
    {
        var protocolData = "*3\r\n$8\r\nREPLCONF\r\n$6\r\nGETACK\r\n$1\r\n*\r\n"u8.ToArray();

        _syncedReplicasCount = 0;
        await Parallel.ForEachAsync(_slaveSockets, cancellationToken, async (slave, token) =>
        {
            try
            {
                await slave.WriteAsync(protocolData, token);
                await slave.FlushAsync(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    public void MarkReplicaAsSynced()
    {
        Interlocked.Increment(ref _syncedReplicasCount);
    }

    public void IncrementWriteCommandOffset(long writeCommandByteLength) =>
        Interlocked.Add(ref _writeCommandOffset, writeCommandByteLength);
}