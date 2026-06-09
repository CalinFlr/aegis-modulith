using System.Net.Http.Headers;
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

    public static TestUser Read(IHeaderDictionary headers)
    {
        var userId = ReadHeader(headers, FakeAuthenticationDefaults.UserIdHeader, TestUsers.Reader.UserId);
        var userName = ReadHeader(headers, FakeAuthenticationDefaults.UserNameHeader, TestUsers.Reader.UserName);
        var roles = ReadCsvHeader(headers, FakeAuthenticationDefaults.RolesHeader, TestUsers.Reader.Roles);
        var scopes = ReadCsvHeader(headers, FakeAuthenticationDefaults.ScopesHeader, TestUsers.Reader.Scopes);
        var permissions = ReadCsvHeader(headers, FakeAuthenticationDefaults.PermissionsHeader, TestUsers.Reader.Permissions);

        return new TestUser(userId, userName, roles, scopes, permissions);
    }

    private static string ReadHeader(IHeaderDictionary headers, string header, string fallback)
    {
        return headers.TryGetValue(header, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : fallback;
    }

    private static IReadOnlyCollection<string> ReadCsvHeader(
        IHeaderDictionary headers,
        string header,
        IReadOnlyCollection<string> fallback)
    {
        if (!headers.TryGetValue(header, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return value.ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
    }
}
