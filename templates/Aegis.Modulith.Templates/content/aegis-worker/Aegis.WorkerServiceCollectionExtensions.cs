using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Worker;

public static class Aegis.WorkerServiceCollectionExtensions
{
    public static IServiceCollection AddAegis.Worker(this IServiceCollection services)
    {
        services.AddHostedService<Aegis.Worker>();
        return services;
    }
}
