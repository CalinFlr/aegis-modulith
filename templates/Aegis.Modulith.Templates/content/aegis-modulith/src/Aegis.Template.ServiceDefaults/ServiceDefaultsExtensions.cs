using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aegis.Template.ServiceDefaults;

public static class ServiceDefaultsExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddServiceDiscoveryPlaceholder();
        builder.Services.AddHttpClient();
        return builder;
    }

    private static IServiceCollection AddServiceDiscoveryPlaceholder(this IServiceCollection services)
    {
        services.AddSingleton(new ServiceDefaultsDescriptor("Aspire-ready service defaults are wired here."));
        return services;
    }
}

public sealed record ServiceDefaultsDescriptor(string Description);
