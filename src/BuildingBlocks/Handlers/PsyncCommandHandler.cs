using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PsyncCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.PsyncCommand;
    
    public Task<byte[]> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes("+FULLRESYNC <REPL_ID> 0\r\n"));
    }
}