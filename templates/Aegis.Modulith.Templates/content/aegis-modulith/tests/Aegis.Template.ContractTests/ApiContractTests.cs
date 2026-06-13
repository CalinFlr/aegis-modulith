using System.Net;
using System.Text.Json;
using Aegis.Template.BuildingBlocks.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aegis.Template.ContractTests;

public sealed class ApiContractTests
{
    [Fact]
    public async Task OpenApi_document_can_be_produced_and_declares_jwt_bearer_security_scheme()
    {
        await using var factory = ContractTestContext.CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");
        var json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(json);

        var bearer = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");

        Assert.Equal("http", bearer.GetProperty("type").GetString());
        Assert.Equal("bearer", bearer.GetProperty("scheme").GetString());
        Assert.Equal("JWT", bearer.GetProperty("bearerFormat").GetString());

        Assert.DoesNotContain("Aegis.Test", json, StringComparison.Ordinal);
        Assert.DoesNotContain("FakeAuthentication", json, StringComparison.Ordinal);
        Assert.DoesNotContain("X-Test-", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Expected_routes_methods_status_codes_and_content_types_are_declared()
    {
        await using var factory = ContractTestContext.CreateFactory();
        using var client = factory.CreateClient();
        using var document = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json"));

        AssertOperation(document, "/", "get", ["200"]);
        AssertOperation(document, "/operations/outbox", "get", ["200"], requiresBearer: true);

#if (sample == "taskhub")
        AssertOperation(document, "/tasks/", "post", ["201"], requestContentType: "application/json", responseContentType: "application/json", requiresBearer: true);
        AssertOperation(document, "/tasks/", "get", ["200"], responseContentType: "application/json", requiresBearer: true);
#else
        AssertOperation(document, "/work-items/", "post", ["201"], requestContentType: "application/json", responseContentType: "application/json", requiresBearer: true);
        AssertOperation(document, "/work-items/{id}", "get", ["200", "404"], responseContentType: "application/json", requiresBearer: true);
#endif

#if (profile == "advanced")
        AssertOperation(document, "/operations/advanced", "get", ["200"], requiresBearer: true);
#endif
    }

    [Fact]
    public void Protected_endpoints_expose_named_permission_policy_metadata()
    {
        using var factory = ContractTestContext.CreateFactory();
        var endpointDataSource = factory.Services.GetRequiredService<EndpointDataSource>();

        AssertEndpointPolicy(endpointDataSource, "/operations/outbox", "GET", AegisAuthorizationPolicies.OperationsRead);

#if (sample == "taskhub")
        AssertEndpointPolicy(endpointDataSource, "/tasks/", "POST", AegisAuthorizationPolicies.TasksWrite);
        AssertEndpointPolicy(endpointDataSource, "/tasks/", "GET", AegisAuthorizationPolicies.TasksRead);
#else
        AssertEndpointPolicy(endpointDataSource, "/work-items/", "POST", AegisAuthorizationPolicies.WorkItemsWrite);
        AssertEndpointPolicy(endpointDataSource, "/work-items/{id:guid}", "GET", AegisAuthorizationPolicies.WorkItemsRead);
#endif

#if (profile == "advanced")
        AssertEndpointPolicy(endpointDataSource, "/operations/advanced", "GET", AegisAuthorizationPolicies.AdvancedRead);
#endif
    }

    [Fact]
    public void Permission_policy_constants_are_registered_as_named_policies()
    {
        using var factory = ContractTestContext.CreateFactory();
        var authorizationOptions = factory.Services.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        foreach (var policyName in ExpectedPermissionPolicies())
        {
            Assert.NotNull(authorizationOptions.GetPolicy(policyName));
        }
    }

    [Fact]
    public void Production_api_contract_does_not_reference_fake_auth_test_infrastructure()
    {
        var forbidden = new[] { "Aegis.Test", "FakeAuthentication", "X-Test-User-Id", "X-Test-Permissions" };
        var failures = new List<string>();

        foreach (var sourceFile in ContractTestContext.SourceFilesUnder(ContractTestContext.ApiProjectRoot))
        {
            var content = File.ReadAllText(sourceFile);
            foreach (var marker in forbidden)
            {
                if (content.Contains(marker, StringComparison.Ordinal))
                {
                    failures.Add($"{ContractTestContext.Relative(sourceFile)} contains test-only auth marker {marker}.");
                }
            }
        }

        Assert.Empty(failures);
    }

    private static IEnumerable<string> ExpectedPermissionPolicies()
    {
        yield return AegisAuthorizationPolicies.WorkItemsRead;
        yield return AegisAuthorizationPolicies.WorkItemsWrite;
        yield return AegisAuthorizationPolicies.TasksRead;
        yield return AegisAuthorizationPolicies.TasksWrite;
        yield return AegisAuthorizationPolicies.OperationsRead;
        yield return AegisAuthorizationPolicies.AdvancedRead;
    }

    private static void AssertEndpointPolicy(EndpointDataSource endpointDataSource, string route, string method, string expectedPolicy)
    {
        var endpoint = endpointDataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Single(candidate =>
                RoutesEqual(candidate.RoutePattern.RawText ?? string.Empty, route) &&
                candidate.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods.Contains(method, StringComparer.OrdinalIgnoreCase) == true);

        var policies = endpoint.Metadata
            .OfType<IAuthorizeData>()
            .Select(metadata => metadata.Policy)
            .Where(policy => !string.IsNullOrWhiteSpace(policy))
            .ToArray();

        Assert.Contains(expectedPolicy, policies);
        Assert.DoesNotContain(policies, policy => policy!.Contains(':', StringComparison.Ordinal));
    }

    private static void AssertOperation(
        JsonDocument document,
        string path,
        string method,
        string[] expectedStatusCodes,
        string? requestContentType = null,
        string? responseContentType = null,
        bool requiresBearer = false)
    {
        var operation = GetOperation(document, path, method);
        var responses = operation.GetProperty("responses");
        foreach (var statusCode in expectedStatusCodes)
        {
            Assert.True(responses.TryGetProperty(statusCode, out _), $"{method.ToUpperInvariant()} {path} should declare {statusCode}.");
        }

        if (requestContentType is not null)
        {
            var requestBodyContent = operation
                .GetProperty("requestBody")
                .GetProperty("content");

            Assert.True(requestBodyContent.TryGetProperty(requestContentType, out _), $"{method.ToUpperInvariant()} {path} should accept {requestContentType}.");
        }

        if (responseContentType is not null)
        {
            var responseContent = responses
                .GetProperty(expectedStatusCodes[0])
                .GetProperty("content");

            Assert.True(responseContent.TryGetProperty(responseContentType, out _), $"{method.ToUpperInvariant()} {path} should produce {responseContentType}.");
        }

        if (requiresBearer)
        {
            Assert.True(operation.TryGetProperty("security", out var security), $"{method.ToUpperInvariant()} {path} should declare bearer security.");
            Assert.Contains(security.EnumerateArray(), requirement => requirement.TryGetProperty("Bearer", out _));
        }
    }

    private static JsonElement GetOperation(JsonDocument document, string path, string method)
    {
        var paths = document.RootElement.GetProperty("paths");
        foreach (var candidate in CandidatePaths(path))
        {
            if (paths.TryGetProperty(candidate, out var pathItem) &&
                pathItem.TryGetProperty(method, out var operation))
            {
                return operation;
            }
        }

        throw new InvalidOperationException($"OpenAPI operation {method.ToUpperInvariant()} {path} was not found.");
    }

    private static IEnumerable<string> CandidatePaths(string path)
    {
        yield return path;

        if (path.Length > 1 && path.EndsWith("/", StringComparison.Ordinal))
        {
            yield return path.TrimEnd('/');
        }
        else if (!path.EndsWith("/", StringComparison.Ordinal))
        {
            yield return $"{path}/";
        }
    }

    private static bool RoutesEqual(string left, string right)
    {
        static string Normalize(string route)
        {
            return route.Length > 1 ? route.TrimEnd('/') : route;
        }

        return string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
    }
}
