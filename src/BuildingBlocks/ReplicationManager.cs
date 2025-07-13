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

    public async Task<RaspProtocolData> GetAcksAsync(CancellationToken cancellationToken)
    {
        var protocolData = "*3\r\n$8\r\nREPLCONF\r\n$6\r\nGETACK\r\n$1\r\n*\r\n"u8.ToArray();

        var replicaResponse = new ConcurrentBag<RaspProtocolData>();
        
        await Parallel.ForEachAsync(_slaveSockets, cancellationToken, async (slave, token) =>
        {
            await slave.WriteAsync(protocolData, token);
            await slave.FlushAsync(token);

            var pars = await RaspProtocolParser.ParseCommand(slave);
            Console.WriteLine(pars);
            
            replicaResponse.Add(pars!);
        });
        
        return replicaResponse.First();
    }
}