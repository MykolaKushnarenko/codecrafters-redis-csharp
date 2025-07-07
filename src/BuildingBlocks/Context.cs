using System.Net.Sockets;

namespace codecrafters_redis.BuildingBlocks;

public class Context
{
    public Socket IncomingSocket { get; set; }
}