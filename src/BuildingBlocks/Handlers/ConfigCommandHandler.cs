using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class ConfigCommandHandler : ICommandHandler<Command>
{
    private readonly ServerConfiguration _configuration;

    public ConfigCommandHandler(ServerConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HandlingCommandName => Constants.ConfigCommand;

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var subCommand = command.Arguments[0].ToString();
        const string dirKey = "dir"; 
        const string dbFileNameKey = "dbfilename";
        
        var list = new List<CommandResult>();
        
        if (subCommand.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
        {
            var key = command.Arguments[1].ToString();
            if (key.Equals(dirKey, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_configuration.Dir))
                {
                    list.Add(BulkStringResult.Create(dirKey));
                    list.Add(BulkStringResult.Create(_configuration.Dir));
                }
            }

            if (key.Equals(dbFileNameKey, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(_configuration.DbFileName))
                {
                    list.Add(BulkStringResult.Create(dbFileNameKey));
                    list.Add(BulkStringResult.Create(dbFileNameKey, _configuration.DbFileName));
                }
            }
        }
        
        return Task.FromResult<CommandResult>(ArrayResult.Create(list.ToArray()));
    }
}