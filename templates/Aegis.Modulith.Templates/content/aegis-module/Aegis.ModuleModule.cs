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
        endpoints.MapGroup("/aegis_module_schema").WithTags(Name);
    }
}
