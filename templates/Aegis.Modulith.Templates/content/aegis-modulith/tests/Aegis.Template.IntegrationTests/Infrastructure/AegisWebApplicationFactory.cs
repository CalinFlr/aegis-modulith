using Aegis.Template.IntegrationTests.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aegis.Template.IntegrationTests.Infrastructure;

public sealed class AegisWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string PostgresConnectionStringKey = "ConnectionStrings:Postgres";
    private const string PostgresConnectionStringEnvironmentKey = "ConnectionStrings__Postgres";
    private const string DefaultPostgresConnectionString =
        "Host=localhost;Port=5432;Database=aegis_template_tests;Username=postgres;Password=postgres";

    private readonly bool enableFakeAuthentication;
    private readonly string? postgresConnectionString;

    private AegisWebApplicationFactory(
        string? postgresConnectionString,
        bool enableFakeAuthentication)
    {
        this.postgresConnectionString = postgresConnectionString;
        this.enableFakeAuthentication = enableFakeAuthentication;
    }

    public static AegisWebApplicationFactory WithPostgres(string? postgresConnectionString = null)
    {
        SetPostgresConnectionString(postgresConnectionString);
        return new AegisWebApplicationFactory(
            postgresConnectionString,
            enableFakeAuthentication: false);
    }

    public static AegisWebApplicationFactory WithFakeAuthentication(string? postgresConnectionString = null)
    {
        SetPostgresConnectionString(postgresConnectionString);
        return new AegisWebApplicationFactory(
            postgresConnectionString,
            enableFakeAuthentication: true);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable(PostgresConnectionStringEnvironmentKey, GetPostgresConnectionString());
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [PostgresConnectionStringKey] = GetPostgresConnectionString()
            });
        });

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

    private string GetPostgresConnectionString()
    {
        return string.IsNullOrWhiteSpace(postgresConnectionString)
            ? DefaultPostgresConnectionString
            : postgresConnectionString;
    }

    private static void SetPostgresConnectionString(string? postgresConnectionString)
    {
        Environment.SetEnvironmentVariable(
            PostgresConnectionStringEnvironmentKey,
            string.IsNullOrWhiteSpace(postgresConnectionString)
                ? DefaultPostgresConnectionString
                : postgresConnectionString);
    }
}
