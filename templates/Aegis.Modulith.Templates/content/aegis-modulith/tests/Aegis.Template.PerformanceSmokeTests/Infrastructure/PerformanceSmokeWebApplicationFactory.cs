#if (sample == "taskhub")
using Aegis.Template.Modules.Modules.Tasks.Infrastructure;
#else
using Aegis.Template.Modules.Modules.WorkItems.Infrastructure;
#endif
using Aegis.Template.PerformanceSmokeTests.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aegis.Template.PerformanceSmokeTests.Infrastructure;

internal sealed class PerformanceSmokeWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"Aegis.Template-performance-smoke-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("PerformanceSmoke");

        builder.ConfigureServices(services =>
        {
            ConfigureInMemoryPersistence(services);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = PerformanceSmokeAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = PerformanceSmokeAuthenticationDefaults.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, PerformanceSmokeAuthenticationHandler>(
                    PerformanceSmokeAuthenticationDefaults.AuthenticationScheme,
                    _ => { });

            services.AddAuthorization();
        });
    }

    private void ConfigureInMemoryPersistence(IServiceCollection services)
    {
        var inMemoryProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

#if (sample == "taskhub")
        services.RemoveAll<DbContextOptions<TasksDbContext>>();
        services.AddDbContext<TasksDbContext>(options => options
            .UseInMemoryDatabase(databaseName)
            .UseInternalServiceProvider(inMemoryProvider));
#else
        services.RemoveAll<DbContextOptions<WorkItemsDbContext>>();
        services.AddDbContext<WorkItemsDbContext>(options => options
            .UseInMemoryDatabase(databaseName)
            .UseInternalServiceProvider(inMemoryProvider));
#endif
    }
}
