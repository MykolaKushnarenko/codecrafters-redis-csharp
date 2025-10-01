using System.Net.Sockets;

namespace DotRedis.BuildingBlocks.Services;

public class SocketAccessor
{
    private readonly AsyncLocal<Socket> _socket = new AsyncLocal<Socket>();

    public Socket GetSocket() => _socket.Value;
    
    public void SetSocket(Socket socket) => _socket.Value = socket;
}