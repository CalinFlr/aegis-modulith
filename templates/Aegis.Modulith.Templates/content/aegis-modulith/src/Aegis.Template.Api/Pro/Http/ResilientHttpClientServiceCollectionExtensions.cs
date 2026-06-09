using Microsoft.Extensions.Http.Resilience;

namespace Aegis.Template.Api.Pro.Http;

public static class ResilientHttpClientServiceCollectionExtensions
{
    public static IServiceCollection AddAegisOutboundHttpClients(this IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(static builder =>
        {
            builder.AddStandardResilienceHandler();
        });

        services.AddHttpClient<SampleExternalStatusClient>(static client =>
        {
            client.BaseAddress = new Uri("https://example.invalid/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
