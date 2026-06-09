using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AegisItemRootNamespace.Modules.Aegis.Module.Infrastructure;

public static class Aegis.ModuleServiceCollectionExtensions
{
    public static IServiceCollection AddAegis.ModuleInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<Aegis.ModuleDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? DefaultConnectionString));

        return services;
    }

    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=aegis_template;Username=postgres;Password=postgres";
}
