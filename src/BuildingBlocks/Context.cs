using System.Net.Sockets;

namespace DotRedis.BuildingBlocks;

public class Context
{
    public NetworkStream IncomingSocket { get; set; }
}