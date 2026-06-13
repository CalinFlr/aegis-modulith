using Aegis.Template.IntegrationTests.Infrastructure;

namespace Aegis.Template.IntegrationTests.Authentication;

public static class AuthenticatedClientExtensions
{
    public static HttpClient CreateAuthenticatedClient(this AegisWebApplicationFactory factory, TestUser? user = null)
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(user ?? TestUsers.Reader);
        return client;
    }

    public static HttpClient AuthenticateAs(this HttpClient client, TestUser user)
    {
        FakeAuthenticationHeaders.Apply(client.DefaultRequestHeaders, user);
        return client;
    }

    public static HttpClient AuthenticateWithPermissions(this HttpClient client, params string[] permissions)
    {
        var user = TestUsers.Reader with { Permissions = permissions, Scopes = permissions };
        return client.AuthenticateAs(user);
    }
}
