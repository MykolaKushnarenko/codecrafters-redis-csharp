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

    public Task<byte[]> HandleAsync(Command command)
    {
        var subCommand = command.Arguments[0].ToString();
        const string dirKey = "dir"; 
        const string dbFileNameKey = "dbfilename";
        
        if (subCommand.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
        {
            var key = command.Arguments[1].ToString();
            var sb = new StringBuilder();
            sb.Append($"*{2}\r\n");
            if (key.Equals(dirKey, StringComparison.CurrentCultureIgnoreCase))
            {
                sb.Append($"${dirKey.Length}{Constants.EOL}{dirKey}{Constants.EOL}");
                if (!string.IsNullOrEmpty(_configuration.Dir))
                {
                    sb.Append($"${_configuration.Dir.Length}{Constants.EOL}{_configuration.Dir}{Constants.EOL}");
                }
                return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
            }

            if (key.Equals(dbFileNameKey, StringComparison.CurrentCultureIgnoreCase))
            {
                sb.Append($"${dbFileNameKey.Length}{dbFileNameKey}{Constants.EOL}");
                if (!string.IsNullOrEmpty(_configuration.DbFileName))
                {
                    sb.Append($"${_configuration.DbFileName.Length}{Constants.EOL}{_configuration.DbFileName}{Constants.EOL}");
                }
                
                return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(Constants.OkResponse));
    }
}