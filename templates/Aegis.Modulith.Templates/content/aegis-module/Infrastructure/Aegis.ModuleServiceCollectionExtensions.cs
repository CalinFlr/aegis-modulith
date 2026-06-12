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
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' is required for the Aegis.Module module.");

        services.AddDbContext<Aegis.ModuleDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
