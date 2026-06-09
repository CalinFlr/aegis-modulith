using Testcontainers.PostgreSql;

namespace Aegis.Template.IntegrationTests.Infrastructure;

/// <summary>
/// Starts PostgreSQL in a Docker container for live integration tests.
/// These tests are intentionally opt-in because Docker availability varies by developer machine and CI agent.
/// </summary>
public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("aegis_template")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
