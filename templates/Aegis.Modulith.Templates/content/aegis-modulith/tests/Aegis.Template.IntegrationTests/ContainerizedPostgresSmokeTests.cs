using Aegis.Template.IntegrationTests.Infrastructure;

namespace Aegis.Template.IntegrationTests;

public sealed class ContainerizedPostgresSmokeTests(PostgresContainerFixture postgres) : IClassFixture<PostgresContainerFixture>
{
    [DockerFact]
    public async Task Api_host_starts_against_containerized_postgres()
    {
        await using var factory = new AegisWebApplicationFactory(postgres.ConnectionString);

        await DatabaseInitialization.InitializeAsync(factory.Services);

        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Aegis.Template", body, StringComparison.Ordinal);
    }
}
