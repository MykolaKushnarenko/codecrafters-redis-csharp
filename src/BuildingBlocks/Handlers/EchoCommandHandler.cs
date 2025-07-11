using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class EchoCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.EchoCommand;
    
    public Task<byte[]> HandleAsync(Command command)
    {
        var response = $"${command.Arguments[0].ToString()!.Length}{Constants.EOL}{command.Arguments[0]}{Constants.EOL}";
        
        return Task.FromResult(Encoding.UTF8.GetBytes(response));
    }
}