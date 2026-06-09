using System.Security.Claims;
using Aegis.Template.IntegrationTests.Authentication;
using Aegis.Template.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.IntegrationTests;

public sealed class FakeAuthenticationSmokeTests
{
    [Fact]
    public async Task Fake_authentication_scheme_maps_test_client_headers_to_claims()
    {
        await using var factory = AegisWebApplicationFactory.WithFakeAuthentication();
        using var client = factory.CreateAuthenticatedClient(TestUsers.Admin);

        Assert.True(client.DefaultRequestHeaders.Contains(FakeAuthenticationDefaults.UserIdHeader));
        Assert.True(client.DefaultRequestHeaders.Contains(FakeAuthenticationDefaults.RolesHeader));
        Assert.True(client.DefaultRequestHeaders.Contains(FakeAuthenticationDefaults.ScopesHeader));

        using var scope = factory.Services.CreateScope();
        var context = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };

        FakeAuthenticationHeaders.Apply(context.Request.Headers, TestUsers.Admin);

        var result = await scope.ServiceProvider
            .GetRequiredService<IAuthenticationService>()
            .AuthenticateAsync(context, FakeAuthenticationDefaults.AuthenticationScheme);

        Assert.True(result.Succeeded);
        Assert.Equal("admin-user", result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Contains(result.Principal!.Claims, claim => claim is { Type: ClaimTypes.Role, Value: "admin" });
        Assert.Contains(result.Principal.Claims, claim => claim is { Type: FakeAuthenticationDefaults.ScopeClaimType, Value: "work-items:write" });
    }
}
