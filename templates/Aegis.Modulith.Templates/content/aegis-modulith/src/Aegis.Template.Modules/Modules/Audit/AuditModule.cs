using Aegis.Template.BuildingBlocks.Modules;
using Aegis.Template.Modules.Modules.Audit.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules.Modules.Audit;

public sealed class AuditModule : IAegisModule
{
    public string Name => "Audit";

    public string Schema => "audit";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres must be configured for the Audit module.");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/audit", () => Results.Ok(new
        {
            module = Name,
            status = "append-only-ready"
        })).WithTags(Name).WithName("ListAuditEntries");
    }
}
