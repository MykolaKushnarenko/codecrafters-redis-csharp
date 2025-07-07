using System.Text;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class EchoCommandHandler : ICommandHandler<EchoCommand>
{
    public Task<byte[]> HandleAsync(EchoCommand command)
    {
        var response = $"${command.Arguments[0].ToString()!.Length}{Constants.EOL}{command.Arguments[0]}{Constants.EOL}";
        
        return Task.FromResult(Encoding.UTF8.GetBytes(response));
    }
}