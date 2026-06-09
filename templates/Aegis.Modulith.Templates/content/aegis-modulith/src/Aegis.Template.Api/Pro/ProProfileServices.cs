using Aegis.Template.Api.Pro.Http;
using Microsoft.AspNetCore.Routing;

namespace Aegis.Template.Api.Pro;

public static class ProProfileServices
{
    public static IServiceCollection AddProProfileServices(this IServiceCollection services)
    {
        services.AddSingleton<Outbox.OutboxDispatcher>();
        services.AddHostedService<Workers.OutboxDispatcherWorker>();
        services.AddSingleton<Idempotency.InMemoryIdempotencyStore>();
        services.AddSingleton<Caching.CacheKeyFactory>();
        services.AddAegisOutboundHttpClients();
        services.AddRateLimiter(options => options.RejectionStatusCode = StatusCodes.Status429TooManyRequests);
        return services;
    }

    public static IEndpointRouteBuilder MapProProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/operations/outbox", (Outbox.OutboxDispatcher dispatcher) => Results.Ok(dispatcher.Describe()))
            .WithName("GetOutboxStatus");

        return endpoints;
    }
}
