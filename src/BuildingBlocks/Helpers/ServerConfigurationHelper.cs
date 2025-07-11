using codecrafters_redis.BuildingBlocks.Configurations;

namespace codecrafters_redis.BuildingBlocks.Helpers;

public static class ServerConfigurationHelper
{
    public static ServerConfiguration CreateConfiguration(string[] args)
    {
        var configuration = new ServerConfiguration();
        
        Console.WriteLine($"{string.Join("," , args)}");
        if (args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("--dir", StringComparison.CurrentCultureIgnoreCase))
                {
                    configuration.Dir = args[i+1];
                }
                if (args[i].Equals("--dbfilename", StringComparison.CurrentCultureIgnoreCase))
                {
                    configuration.DbFileName = args[i+1];
                }

                if (args[i].Equals("--port", StringComparison.CurrentCultureIgnoreCase))
                {
                    configuration.Port = int.Parse(args[i+1]);
                }
            }
        }

        return configuration;
    }
}