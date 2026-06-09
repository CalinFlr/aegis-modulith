using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Aegis.Template.IntegrationTests.Infrastructure;

public sealed class AegisWebApplicationFactory(string? postgresConnectionString = null) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        if (!string.IsNullOrWhiteSpace(postgresConnectionString))
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] = postgresConnectionString
                });
            });
        }
    }
}
