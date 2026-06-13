using System.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Aegis.Template.IntegrationTests.Authentication;

public static class FakeAuthenticationHeaders
{
    public static void Apply(HttpRequestHeaders headers, TestUser user)
    {
        headers.Remove(FakeAuthenticationDefaults.UserIdHeader);
        headers.Remove(FakeAuthenticationDefaults.UserNameHeader);
        headers.Remove(FakeAuthenticationDefaults.RolesHeader);
        headers.Remove(FakeAuthenticationDefaults.ScopesHeader);
        headers.Remove(FakeAuthenticationDefaults.PermissionsHeader);

        headers.TryAddWithoutValidation(FakeAuthenticationDefaults.UserIdHeader, user.UserId);
        headers.TryAddWithoutValidation(FakeAuthenticationDefaults.UserNameHeader, user.UserName);
        headers.TryAddWithoutValidation(FakeAuthenticationDefaults.RolesHeader, string.Join(",", user.Roles));
        headers.TryAddWithoutValidation(FakeAuthenticationDefaults.ScopesHeader, string.Join(",", user.Scopes));
        headers.TryAddWithoutValidation(FakeAuthenticationDefaults.PermissionsHeader, string.Join(",", user.Permissions));
    }

    public static void Apply(IHeaderDictionary headers, TestUser user)
    {
        headers[FakeAuthenticationDefaults.UserIdHeader] = user.UserId;
        headers[FakeAuthenticationDefaults.UserNameHeader] = user.UserName;
        headers[FakeAuthenticationDefaults.RolesHeader] = string.Join(",", user.Roles);
        headers[FakeAuthenticationDefaults.ScopesHeader] = string.Join(",", user.Scopes);
        headers[FakeAuthenticationDefaults.PermissionsHeader] = string.Join(",", user.Permissions);
    }

    public static bool TryRead(IHeaderDictionary headers, [NotNullWhen(true)] out TestUser? user)
    {
        var userId = ReadHeader(headers, FakeAuthenticationDefaults.UserIdHeader);
        if (string.IsNullOrWhiteSpace(userId))
        {
            user = null;
            return false;
        }

        var userName = ReadHeader(headers, FakeAuthenticationDefaults.UserNameHeader) ?? userId;
        var roles = ReadCsvHeader(headers, FakeAuthenticationDefaults.RolesHeader);
        var scopes = ReadCsvHeader(headers, FakeAuthenticationDefaults.ScopesHeader);
        var permissions = ReadCsvHeader(headers, FakeAuthenticationDefaults.PermissionsHeader);

        user = new TestUser(userId, userName, roles, scopes, permissions);
        return true;
    }

    private static string? ReadHeader(IHeaderDictionary headers, string header)
    {
        return headers.TryGetValue(header, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : null;
    }

    private static IReadOnlyCollection<string> ReadCsvHeader(
        IHeaderDictionary headers,
        string header)
    {
        if (!headers.TryGetValue(header, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
    }
}
