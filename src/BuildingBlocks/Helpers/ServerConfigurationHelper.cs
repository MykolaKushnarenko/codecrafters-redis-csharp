using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.Configurations;

namespace DotRedis.BuildingBlocks.Helpers;

public static class ServerConfigurationHelper
{
    public static ServerConfiguration CreateConfiguration(string[] args)
    {
        var configuration = new ServerConfiguration();
        
        if (args.Length <= 0) return configuration;
        
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(Constants.DirArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                configuration.Dir = args[i+1];
            }
            if (args[i].Equals(Constants.DbFileNameArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                configuration.DbFileName = args[i+1];
            }

            if (args[i].Equals(Constants.PortArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                configuration.Port = int.Parse(args[i+1]);
                Console.WriteLine(configuration.Port);
            }

            if (args[i].Equals(Constants.ReplicaOfArgument, StringComparison.CurrentCultureIgnoreCase))
            {
                var masterHostConfiguration = args[i+1].Split(" ");
                    
                configuration.MasterHost = masterHostConfiguration[0];
                configuration.MasterPort = int.Parse(masterHostConfiguration[1]);
                configuration.Role = "slave";
            }
        }

        return configuration;
    }
    
    
}