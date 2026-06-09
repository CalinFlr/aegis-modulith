using Aegis.Template.BuildingBlocks.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Aegis.Template.Api.Pro.Auth;

public static class AegisPermissionPolicyServiceCollectionExtensions
{
    public static IServiceCollection AddAegisPermissionPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            AddPermissionPolicy(options, AegisAuthorizationPolicies.WorkItemsRead, AegisPermissions.WorkItemsRead);
            AddPermissionPolicy(options, AegisAuthorizationPolicies.WorkItemsWrite, AegisPermissions.WorkItemsWrite);
            AddPermissionPolicy(options, AegisAuthorizationPolicies.TasksRead, AegisPermissions.TasksRead);
            AddPermissionPolicy(options, AegisAuthorizationPolicies.TasksWrite, AegisPermissions.TasksWrite);
            AddPermissionPolicy(options, AegisAuthorizationPolicies.OperationsRead, AegisPermissions.OperationsRead);
            AddPermissionPolicy(options, AegisAuthorizationPolicies.AdvancedRead, AegisPermissions.AdvancedRead);
        });

        return services;
    }

    private static void AddPermissionPolicy(AuthorizationOptions options, string policyName, string permission)
    {
        options.AddPolicy(policyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasClaim(AegisPermissionClaimTypes.Permission, permission) ||
                context.User.HasClaim(AegisPermissionClaimTypes.Scope, permission));
        });
    }
}
