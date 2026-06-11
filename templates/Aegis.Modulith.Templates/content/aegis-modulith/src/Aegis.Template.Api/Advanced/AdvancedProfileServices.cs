using Microsoft.AspNetCore.Routing;

namespace Aegis.Template.Api.Advanced;

public static class AdvancedProfileServices
{
    public static IServiceCollection AddAdvancedProfileServices(this IServiceCollection services)
    {
        services.AddScoped<Permissions.PermissionEvaluator>();
        services.AddScoped<Tenancy.TenantContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapAdvancedProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/operations/advanced", () => Results.Ok(new
        {
            permissions = "skeleton",
            tenancy = "skeleton",
            deployment = "ready"
        })).WithName("GetAdvancedProfileStatus");

        return endpoints;
    }
}
