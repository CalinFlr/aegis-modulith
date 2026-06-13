namespace Aegis.Template.IntegrationTests.Authentication;

public static class FakeAuthenticationDefaults
{
    public const string AuthenticationScheme = "Aegis.Test";
    public const string UserIdHeader = "X-Test-User-Id";
    public const string UserNameHeader = "X-Test-User-Name";
    public const string RolesHeader = "X-Test-Roles";
    public const string ScopesHeader = "X-Test-Scopes";
    public const string ScopeClaimType = "scope";
}
