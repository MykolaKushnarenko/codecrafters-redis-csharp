using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Helpers;

public static class ServerConfigurationHelper
{
    public static ServerConfiguration CreateConfiguration(string[] args)
    {
        var configuration = new ServerConfiguration();
        
        if (args.Length > 0)
        {
            string previous = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                if (previous.Equals("--dir", StringComparison.CurrentCultureIgnoreCase))
                {
                    configuration.Dir = args[i++];
                }
                if (previous.Equals("--dbfilename", StringComparison.CurrentCultureIgnoreCase))
                {
                    configuration.DbFileName = args[i++];
                }
                previous = args[i++];
            }
        }

        return configuration;
    }
}