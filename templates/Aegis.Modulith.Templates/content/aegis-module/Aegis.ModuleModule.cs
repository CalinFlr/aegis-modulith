using System.Reflection;
using AegisBuildingBlocksNamespace.Modules;
using AegisItemRootNamespace.Modules.Aegis.Module.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AegisItemRootNamespace.Modules.Aegis.Module;

public sealed class Aegis.ModuleModule : IAegisModule
{
    public string Name => "Aegis.Module";

    public string Schema => Aegis.ModuleDbContext.Schema;

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAegis.ModuleInfrastructure(configuration);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/aegis_module_schema").WithTags(Name);
        MapFeatureEndpoints(group);
    }

    private static void MapFeatureEndpoints(RouteGroupBuilder group)
    {
        var featureNamespacePrefix = typeof(Aegis.ModuleModule).Namespace + ".Features.";
        var endpointMethods = typeof(Aegis.ModuleModule).Assembly.GetTypes()
            .Where(type => type is { IsAbstract: true, IsSealed: true } &&
                type.Namespace?.StartsWith(featureNamespacePrefix, StringComparison.Ordinal) == true)
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(method =>
            {
                var parameters = method.GetParameters();
                return method.Name.StartsWith("Map", StringComparison.Ordinal) &&
                    method.ReturnType == typeof(RouteGroupBuilder) &&
                    parameters.Length == 1 &&
                    parameters[0].ParameterType == typeof(RouteGroupBuilder);
            })
            .OrderBy(method => method.DeclaringType?.FullName, StringComparer.Ordinal)
            .ThenBy(method => method.Name, StringComparer.Ordinal);

        foreach (var method in endpointMethods)
        {
            method.Invoke(null, new object[] { group });
        }
    }
}
