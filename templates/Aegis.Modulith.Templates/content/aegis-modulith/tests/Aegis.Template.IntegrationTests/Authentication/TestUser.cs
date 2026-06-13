namespace Aegis.Template.IntegrationTests.Authentication;

public sealed record TestUser(
    string UserId,
    string UserName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Scopes);
