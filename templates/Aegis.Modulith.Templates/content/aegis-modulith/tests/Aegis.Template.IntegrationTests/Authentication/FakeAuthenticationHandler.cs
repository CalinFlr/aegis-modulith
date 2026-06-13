using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aegis.Template.IntegrationTests.Authentication;

public sealed class FakeAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!FakeAuthenticationHeaders.TryRead(Request.Headers, out var user))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Name, user.UserName)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Scopes.Select(scope => new Claim(FakeAuthenticationDefaults.ScopeClaimType, scope)));
        claims.AddRange(user.Permissions.Select(permission => new Claim(FakeAuthenticationDefaults.PermissionClaimType, permission)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
