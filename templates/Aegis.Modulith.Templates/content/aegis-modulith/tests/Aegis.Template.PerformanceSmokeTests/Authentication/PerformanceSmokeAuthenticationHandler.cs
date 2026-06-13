using System.Security.Claims;
using System.Text.Encodings.Web;
using Aegis.Template.BuildingBlocks.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aegis.Template.PerformanceSmokeTests.Authentication;

internal sealed class PerformanceSmokeAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "performance-smoke-user"),
            new(ClaimTypes.Name, "Performance Smoke User")
        };

        foreach (var permission in RequestedPermissions())
        {
            claims.Add(new Claim(AegisPermissionClaimTypes.Permission, permission));
            claims.Add(new Claim(AegisPermissionClaimTypes.Scope, permission));
        }

        var identity = new ClaimsIdentity(claims, PerformanceSmokeAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, PerformanceSmokeAuthenticationDefaults.AuthenticationScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private IEnumerable<string> RequestedPermissions()
    {
        if (!Request.Headers.TryGetValue(PerformanceSmokeAuthenticationDefaults.PermissionsHeader, out var values))
        {
            yield break;
        }

        foreach (var value in values.ToString().Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            yield return value;
        }
    }
}
