using codecrafters_redis.BuildingBlocks;
using codecrafters_redis.BuildingBlocks.Extensions;
using Microsoft.Extensions.DependencyInjection;

// You can use print statements as follows for debugging, they'll be visible when running tests.

var serviceCollection = new ServiceCollection();
var provider = serviceCollection.AddBuildingBlocks(args).BuildServiceProvider();

var server = provider.GetRequiredService<Server>();
await server.RunAsync();