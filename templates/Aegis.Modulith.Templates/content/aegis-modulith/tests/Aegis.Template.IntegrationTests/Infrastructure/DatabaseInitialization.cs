using Aegis.Template.Modules.Modules.WorkItems.Infrastructure;
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
        var dbContext = scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();
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
