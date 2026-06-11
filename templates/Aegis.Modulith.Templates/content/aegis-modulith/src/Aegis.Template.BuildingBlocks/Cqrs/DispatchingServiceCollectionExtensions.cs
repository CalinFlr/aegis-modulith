using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

#if AEGIS_MEDIATR
using MediatR;
#endif

namespace Aegis.Template.BuildingBlocks.Cqrs;

public static class DispatchingServiceCollectionExtensions
{
    public static IServiceCollection AddAegisDispatching(this IServiceCollection services, params Assembly[] handlerAssemblies)
    {
#if AEGIS_MEDIATR
        services.AddMediatR(configuration =>
        {
            foreach (var assembly in handlerAssemblies)
            {
                configuration.RegisterServicesFromAssembly(assembly);
            }
        });
        services.AddScoped<ICommandDispatcher, MediatRCommandDispatcher>();
        services.AddScoped<IQueryDispatcher, MediatRQueryDispatcher>();
#endif

#if AEGIS_CORE_MEDIATOR
        foreach (var assembly in handlerAssemblies)
        {
            RegisterCoreHandlers(services, assembly);
        }

        services.AddScoped<ICommandDispatcher, ServiceProviderCommandDispatcher>();
        services.AddScoped<IQueryDispatcher, ServiceProviderQueryDispatcher>();
#endif

        return services;
    }

#if AEGIS_CORE_MEDIATOR
    private static void RegisterCoreHandlers(IServiceCollection services, Assembly assembly)
    {
        foreach (var implementationType in assembly.GetTypes().Where(type => type is { IsClass: true, IsAbstract: false }))
        {
            var handlerInterfaces = implementationType
                .GetInterfaces()
                .Where(type => type.IsGenericType)
                .Where(type =>
                    type.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                    type.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, implementationType);
            }
        }
    }

    private sealed class ServiceProviderCommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
            dynamic handler = serviceProvider.GetRequiredService(handlerType);
            return handler.Handle((dynamic)command, cancellationToken);
        }
    }

    private sealed class ServiceProviderQueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            dynamic handler = serviceProvider.GetRequiredService(handlerType);
            return handler.Handle((dynamic)query, cancellationToken);
        }
    }
#endif

#if AEGIS_MEDIATR
    private sealed class MediatRCommandDispatcher(ISender sender) : ICommandDispatcher
    {
        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            if (command is not IRequest<TResponse> request)
            {
                throw new InvalidOperationException($"{command.GetType().Name} must implement MediatR.IRequest<{typeof(TResponse).Name}> in MediatR mode.");
            }

            return sender.Send(request, cancellationToken);
        }
    }

    private sealed class MediatRQueryDispatcher(ISender sender) : IQueryDispatcher
    {
        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            if (query is not IRequest<TResponse> request)
            {
                throw new InvalidOperationException($"{query.GetType().Name} must implement MediatR.IRequest<{typeof(TResponse).Name}> in MediatR mode.");
            }

            return sender.Send(request, cancellationToken);
        }
    }
#endif
}
