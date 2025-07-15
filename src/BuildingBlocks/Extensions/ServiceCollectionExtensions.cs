using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Communication;
using codecrafters_redis.BuildingBlocks.HandlerFactory;
using codecrafters_redis.BuildingBlocks.Handlers;
using codecrafters_redis.BuildingBlocks.Helpers;
using codecrafters_redis.BuildingBlocks.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace codecrafters_redis.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, string[] args)
    {
        services.AddSingleton<Server>();
        services.AddSingleton<ApplicationLifetime>();
        
        services.AddSingleton<InMemoryStorage>();
        services.AddSingleton<WatchDog>();
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddSingleton<IMasterClient, MasterClient>();
        services.AddSingleton<ReplicationManager>();
        services.AddSingleton<AcknowledgeCommandTracker>();
        services.AddSingleton<StreamInMemoryStorage>();

        services.Scan(x =>
            x.FromAssemblyOf<Command>()
                .AddClasses(c => c.AssignableTo<ICommandHandler<Command>>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        var serverConfiguration = ServerConfigurationHelper.CreateConfiguration(args);
        services.AddSingleton(serverConfiguration);

        services.AddSingleton<Initiator>();
        
        return services;
    }
}