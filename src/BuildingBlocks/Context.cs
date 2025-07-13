using System.Net.Sockets;

namespace codecrafters_redis.BuildingBlocks;

public class Context
{
    public NetworkStream IncomingSocket { get; set; }
}