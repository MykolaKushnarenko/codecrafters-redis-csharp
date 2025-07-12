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

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();

        if (subCommand.Equals("replication", StringComparison.CurrentCultureIgnoreCase))
        {
            var sb = new StringBuilder();
            
            sb.Append($"role:{_configuration.Role}");
            sb.Append(Constants.EOL);
            sb.Append("master_replid:8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb"); // for now just hardcode
            sb.Append(Constants.EOL);
            sb.Append("master_repl_offset:0"); // for now just hardcode
            
            return Task.FromResult<CommandResult>(BulkStringResult.Create(sb.ToString()));

        }
        
        return Task.FromResult<CommandResult>(ErrorResult.Create($"unknown command {subCommand}"));
    }
}