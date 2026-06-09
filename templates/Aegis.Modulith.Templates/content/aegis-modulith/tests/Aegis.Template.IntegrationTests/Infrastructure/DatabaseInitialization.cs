namespace Aegis.Template.IntegrationTests.Infrastructure;

public static class DatabaseInitialization
{
    public static Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Starter modules do not generate EF Core migrations yet. When a module adds migrations,
        // replace this placeholder with Database.MigrateAsync calls for each module DbContext.
        return Task.CompletedTask;
    }
}
