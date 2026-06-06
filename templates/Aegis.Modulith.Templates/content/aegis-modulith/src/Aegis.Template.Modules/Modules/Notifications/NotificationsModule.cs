using Aegis.Template.BuildingBlocks.Modules;
using Aegis.Template.Modules.Modules.Notifications.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules.Modules.Notifications;

public sealed class NotificationsModule : IAegisModule
{
    public string Name => "Notifications";

    public string Schema => "notifications";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? DefaultConnectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/notifications", () => Results.Ok(new
        {
            module = Name,
            status = "queue-ready"
        })).WithTags(Name).WithName("ListNotifications");
    }

    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=aegis_template;Username=postgres;Password=postgres";
}
