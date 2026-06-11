using System.Reflection;
using Aegis.Template.BuildingBlocks.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules;

public static class ModuleRegistrationExtensions
{
    public static IServiceCollection AddAegisModules(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in DiscoverModules())
        {
            module.AddServices(services, configuration);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapAegisModules(this IEndpointRouteBuilder endpoints)
    {
        foreach (var module in DiscoverModules())
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }

    private static IReadOnlyList<IAegisModule> DiscoverModules()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IAegisModule).IsAssignableFrom(type))
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Select(type => (IAegisModule)Activator.CreateInstance(type)!)
            .OrderBy(module => module.Name, StringComparer.Ordinal)
            .ToArray();
    }
}
