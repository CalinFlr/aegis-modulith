using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public static class InboxServiceCollectionExtensions
{
    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=aegis_template;Username=postgres;Password=postgres";

    public static IServiceCollection AddAegisInbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AegisInboxDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? DefaultConnectionString));

        services.AddScoped<IInboxStore, EfCoreInboxStore>();
        services.AddScoped<InboxProcessor>();
        services.AddScoped<IInboxMessageHandler, SampleIntegrationEventInboxHandler>();

        if (configuration.GetValue<bool>("Inbox:EnableBackgroundProcessor"))
        {
            services.AddHostedService<InboxProcessorWorker>();
        }

        return services;
    }
}
