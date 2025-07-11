using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class RepconfCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.RepconfCommand;
    
    public Task<byte[]> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("listening-port", StringComparison.CurrentCultureIgnoreCase) || 
            subCommand.Equals("capa", StringComparison.CurrentCultureIgnoreCase))
        {
            return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));;
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes($"-ERR unknown sub-command {subCommand}\r\n"));
    }
}