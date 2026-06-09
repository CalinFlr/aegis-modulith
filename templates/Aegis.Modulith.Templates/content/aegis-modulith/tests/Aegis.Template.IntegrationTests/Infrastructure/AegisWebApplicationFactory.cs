using Aegis.Template.IntegrationTests.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.IntegrationTests.Infrastructure;

public sealed class AegisWebApplicationFactory(
    string? postgresConnectionString = null,
    bool enableFakeAuthentication = false) : WebApplicationFactory<Program>
{
    public static AegisWebApplicationFactory WithFakeAuthentication(string? postgresConnectionString = null)
    {
        return new AegisWebApplicationFactory(postgresConnectionString, enableFakeAuthentication: true);
    }

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

        if (enableFakeAuthentication)
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = FakeAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = FakeAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                        FakeAuthenticationDefaults.AuthenticationScheme,
                        _ => { });

                services.AddAuthorization();
            });
        }
    }
}
