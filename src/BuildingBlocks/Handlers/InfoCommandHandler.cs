using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class InfoCommandHandler : ICommandHandler<Command>
{
    private readonly ServerConfiguration _configuration;

    public InfoCommandHandler(ServerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HandlingCommandName => Constants.InfoCommand;

    public Task<byte[]> HandleAsync(Command command)
    {
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("replication", StringComparison.CurrentCultureIgnoreCase))
        {
            var roleReply = $"role:{_configuration.Role}";
            var reply = $"${roleReply.Length}{Constants.EOL}{roleReply}{Constants.EOL}";
            return Task.FromResult(Encoding.UTF8.GetBytes(reply));
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));
    }
}