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
            var roleReply = $"role:{_configuration.Role}{Constants.EOL}";
            var replicationIdReply = "master_replid:8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb"; // for now just hardcode
            var replicationOffsetReply = $"master_repl_offset:0{Constants.EOL}"; // for now just hardcode
            var reply =
                $"${roleReply.Length + replicationIdReply.Length + replicationOffsetReply.Length}{Constants.EOL}" +
                $"{roleReply}" +
                $"{replicationOffsetReply}"+
                $"{replicationIdReply}{Constants.EOL}";
            
            return Task.FromResult(Encoding.UTF8.GetBytes(reply));
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));
    }
}