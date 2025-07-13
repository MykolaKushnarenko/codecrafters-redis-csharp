using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Extensions;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
var provider = serviceCollection.AddBuildingBlocks(args).BuildServiceProvider();

var server = provider.GetRequiredService<Server>();
await server.RunAsync();