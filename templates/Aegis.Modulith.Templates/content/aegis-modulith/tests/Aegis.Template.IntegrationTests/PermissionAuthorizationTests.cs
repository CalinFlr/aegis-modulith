using System.Net;
using Aegis.Template.IntegrationTests.Authentication;
using Aegis.Template.IntegrationTests.Infrastructure;

namespace Aegis.Template.IntegrationTests;

public sealed class PermissionAuthorizationTests
{
    [Fact]
    public async Task Request_with_required_permission_can_access_protected_endpoint()
    {
        await using var factory = AegisWebApplicationFactory.WithFakeAuthentication();
        using var client = factory.CreateAuthenticatedClient(TestUsers.OperationsReader);

        var response = await client.GetAsync("/operations/outbox");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Request_without_required_permission_is_forbidden()
    {
        await using var factory = AegisWebApplicationFactory.WithFakeAuthentication();
        using var client = factory.CreateAuthenticatedClient(TestUsers.WithoutPermissions);

        var response = await client.GetAsync("/operations/outbox");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
