using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Communication;
using DotRedis.BuildingBlocks.HandlerFactory;
using DotRedis.BuildingBlocks.Handlers;
using DotRedis.BuildingBlocks.Helpers;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DotRedis.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    private static List<string> _excludedHandlersFromDecoration =
    [
        nameof(MultiCommandHandler), 
        nameof(ExecCommandHandler), 
        nameof(TransactionCommandHandlerDecorator),
        nameof(DiscardCommandHandler)
    ];
    
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services, string[] args)
    {
        services.AddSingleton<Server>();
        services.AddSingleton<ApplicationLifetime>();
        
        services.AddSingleton<WatchDog>();
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddSingleton<IMasterClient, MasterClient>();
        services.AddSingleton<ReplicationManager>();
        services.AddSingleton<AcknowledgeCommandTracker>();
        services.AddSingleton<RedisStorage>();
        services.AddSingleton<RedisStreamListener>();
        services.AddSingleton<TransactionManager>();

        services.Scan(x =>
            x.FromAssemblyOf<Command>()
                .AddClasses(c => c.AssignableTo<ICommandHandler<Command>>().Where(t => !_excludedHandlersFromDecoration.Contains(t.Name)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        
        services.Decorate<ICommandHandler<Command>, TransactionCommandHandlerDecorator>();
        services.AddSingleton<ICommandHandler<Command>, MultiCommandHandler>();
        services.AddSingleton<ICommandHandler<Command>, ExecCommandHandler>();
        services.AddSingleton<ICommandHandler<Command>, DiscardCommandHandler>();

        var serverConfiguration = ServerConfigurationHelper.CreateConfiguration(args);
        services.AddSingleton(serverConfiguration);

        services.AddSingleton<Initiator>();
        
        return services;
    }
}