using codecrafters_redis.BuildingBlocks.Handlers;
using codecrafters_redis.BuildingBlocks.Helpers;
using codecrafters_redis.BuildingBlocks.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace codecrafters_redis.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, string[] args)
    {
        services.AddSingleton<InMemoryStorage>();
        services.AddSingleton<WatchDog>();
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<PingCommandHandler>();
        services.AddSingleton<EchoCommandHandler>();
        services.AddSingleton<SetCommandHandler>();
        services.AddSingleton<GetCommandHandler>();
        services.AddSingleton<ConfigCommandHandler>();

        var serverConfiguration = ServerConfigurationHelper.CreateConfiguration(args);
        services.AddSingleton(serverConfiguration);

        return services;
    }
}