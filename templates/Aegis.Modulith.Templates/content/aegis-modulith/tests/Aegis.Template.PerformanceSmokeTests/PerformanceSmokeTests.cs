using System.Net;
using Aegis.Template.BuildingBlocks.Authorization;
using Aegis.Template.PerformanceSmokeTests.Authentication;
using Aegis.Template.PerformanceSmokeTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aegis.Template.PerformanceSmokeTests;

public sealed class PerformanceSmokeTests
{
    [Fact]
    public async Task Api_test_host_startup_smoke_stays_within_loose_threshold()
    {
        var elapsed = await PerformanceSmokeAssertions.MeasureOnceAsync("API test host startup", async () =>
        {
            await using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            using var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        });

        PerformanceSmokeAssertions.AssertWithin("API test host startup", elapsed, PerformanceSmokeThresholds.HostStartup);
    }

    [Fact]
    public async Task Health_endpoint_response_smoke_stays_within_loose_threshold()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var samples = await PerformanceSmokeAssertions.MeasureWarmedSamplesAsync(() => client.GetAsync("/health"));

        PerformanceSmokeAssertions.AssertBestSampleWithin("Health endpoint response", samples, PerformanceSmokeThresholds.SimpleRequest);
    }

    [Fact]
    public async Task Authenticated_request_path_smoke_stays_within_loose_threshold()
    {
        await using var factory = new PerformanceSmokeWebApplicationFactory();
        using var client = factory.CreateClient();
        AddPermissions(client, AegisPermissions.OperationsRead);

        var samples = await PerformanceSmokeAssertions.MeasureWarmedSamplesAsync(() => client.GetAsync("/operations/outbox"));

        PerformanceSmokeAssertions.AssertBestSampleWithin("Authenticated operations request", samples, PerformanceSmokeThresholds.AuthenticatedRequest);
    }

    [Fact]
    public async Task Cqrs_dispatch_request_path_smoke_stays_within_loose_threshold()
    {
        await using var factory = new PerformanceSmokeWebApplicationFactory();
        using var client = factory.CreateClient();

#if (sample == "taskhub")
        AddPermissions(client, AegisPermissions.TasksRead);
        var samples = await PerformanceSmokeAssertions.MeasureWarmedSamplesAsync(() => client.GetAsync("/tasks/"));
#else
        AddPermissions(client, AegisPermissions.WorkItemsRead);
        var samples = await PerformanceSmokeAssertions.MeasureWarmedSamplesAsync(() => client.GetAsync($"/work-items/{Guid.NewGuid():D}"));
#endif

        PerformanceSmokeAssertions.AssertBestSampleWithin("CQRS query dispatch request", samples, PerformanceSmokeThresholds.CqrsDispatchRequest);
    }

    [Fact]
    public async Task OpenApi_document_generation_smoke_stays_within_loose_threshold()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var samples = await PerformanceSmokeAssertions.MeasureWarmedSamplesAsync(() => client.GetAsync("/openapi/v1.json"));

        PerformanceSmokeAssertions.AssertBestSampleWithin("OpenAPI document generation", samples, PerformanceSmokeThresholds.OpenApiGeneration);
    }

    private static void AddPermissions(HttpClient client, params string[] permissions)
    {
        client.DefaultRequestHeaders.Remove(PerformanceSmokeAuthenticationDefaults.PermissionsHeader);
        client.DefaultRequestHeaders.Add(PerformanceSmokeAuthenticationDefaults.PermissionsHeader, string.Join(' ', permissions));
    }
}
