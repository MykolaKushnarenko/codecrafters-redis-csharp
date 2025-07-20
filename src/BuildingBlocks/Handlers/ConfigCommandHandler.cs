using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Configurations;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the "CONFIG" command in a Redis-like system, providing support for
///     retrieving or managing server configuration settings.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/commands/config-get/
/// </remarks>
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