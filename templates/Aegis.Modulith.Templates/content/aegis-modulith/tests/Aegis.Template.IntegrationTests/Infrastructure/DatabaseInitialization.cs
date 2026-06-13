using Aegis.Template.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.IntegrationTests.Infrastructure;

public static class DatabaseInitialization
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Starter modules do not generate EF Core migrations yet. When a module adds migrations,
        // replace this placeholder with Database.MigrateAsync calls for each module DbContext.
        using var scope = services.CreateScope();
        var dbContextType = typeof(ModuleRegistrationExtensions).Assembly
            .GetTypes()
            .Where(type => typeof(DbContext).IsAssignableFrom(type))
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .First();

        var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
        try
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
            await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
