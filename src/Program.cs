using DotRedis.BuildingBlocks;
using DotRedis.BuildingBlocks.Extensions;
using Microsoft.Extensions.DependencyInjection;

try
{
    var serviceCollection = new ServiceCollection();
    var provider = serviceCollection.AddBuildingBlocks(args).BuildServiceProvider();
    
    var server = provider.GetRequiredService<Server>();
    await server.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
