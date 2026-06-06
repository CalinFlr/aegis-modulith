namespace Aegis.Template.Api.Advanced.Permissions;

public sealed class PermissionEvaluator
{
    public bool IsAllowed(string permission) => !string.IsNullOrWhiteSpace(permission);
}
