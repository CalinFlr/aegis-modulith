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
        services.AddDbContext<AuditDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? DefaultConnectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/audit", () => Results.Ok(new
        {
            module = Name,
            status = "append-only-ready"
        })).WithTags(Name).WithName("ListAuditEntries");
    }

    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=aegis_template;Username=postgres;Password=postgres";
}
