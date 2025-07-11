using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PsyncCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.PsyncCommand;
    
    public Task<byte[]> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes("+FULLRESYNC 8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb 0\r\n"));
    }
}