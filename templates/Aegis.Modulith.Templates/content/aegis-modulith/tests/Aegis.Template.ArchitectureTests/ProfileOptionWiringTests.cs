namespace Aegis.Template.ArchitectureTests;

public sealed class ProfileOptionWiringTests
{
    [Fact]
    public void Profile_wiring_matches_selected_profile()
    {
        var profile = ArchitectureTestContext.GetOption("AegisProfile");
        var program = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "Program.cs");
        var apiProject = Path.Combine(ArchitectureTestContext.ApiProjectRoot, $"{ArchitectureTestContext.ProjectPrefix}.Api.csproj");
        var solution = Path.Combine(ArchitectureTestContext.SolutionRoot, $"{ArchitectureTestContext.ProjectPrefix}.sln");
        var appHostProject = Path.Combine(ArchitectureTestContext.SourceRoot, $"{ArchitectureTestContext.ProjectPrefix}.AppHost", $"{ArchitectureTestContext.ProjectPrefix}.AppHost.csproj");
        var serviceDefaultsProject = Path.Combine(ArchitectureTestContext.SourceRoot, $"{ArchitectureTestContext.ProjectPrefix}.ServiceDefaults", $"{ArchitectureTestContext.ProjectPrefix}.ServiceDefaults.csproj");
        var dockerfile = Path.Combine(ArchitectureTestContext.SolutionRoot, "Dockerfile");

        if (profile == "core")
        {
            Assert.False(File.Exists(appHostProject), "core profile must not include AppHost.");
            Assert.False(File.Exists(serviceDefaultsProject), "core profile must not include ServiceDefaults.");
            Assert.False(File.Exists(dockerfile), "core profile must not include Dockerfile.");
            Assert.DoesNotContain("ServiceDefaults", File.ReadAllText(apiProject), StringComparison.Ordinal);
            Assert.DoesNotContain("AppHost", File.ReadAllText(solution), StringComparison.Ordinal);
            Assert.DoesNotContain("AddProProfileServices", File.ReadAllText(program), StringComparison.Ordinal);
            Assert.DoesNotContain("AddAdvancedProfileServices", File.ReadAllText(program), StringComparison.Ordinal);
            return;
        }

        Assert.True(File.Exists(appHostProject), $"{profile} profile must include AppHost.");
        Assert.True(File.Exists(serviceDefaultsProject), $"{profile} profile must include ServiceDefaults.");
        Assert.True(File.Exists(dockerfile), $"{profile} profile must include Dockerfile.");
        Assert.Contains("ServiceDefaults", File.ReadAllText(apiProject), StringComparison.Ordinal);
        Assert.Contains("AppHost", File.ReadAllText(solution), StringComparison.Ordinal);

        var programContent = File.ReadAllText(program);
        Assert.Contains("builder.AddServiceDefaults();", programContent, StringComparison.Ordinal);
        Assert.Contains("builder.Services.AddProProfileServices();", programContent, StringComparison.Ordinal);
        Assert.Contains("app.UseRateLimiter();", programContent, StringComparison.Ordinal);
        Assert.Contains("app.MapProProfileEndpoints();", programContent, StringComparison.Ordinal);

        if (profile == "advanced")
        {
            Assert.Contains("builder.Services.AddAdvancedProfileServices();", programContent, StringComparison.Ordinal);
            Assert.Contains("app.MapAdvancedProfileEndpoints();", programContent, StringComparison.Ordinal);
        }
        else
        {
            Assert.DoesNotContain("AddAdvancedProfileServices", programContent, StringComparison.Ordinal);
            Assert.DoesNotContain("MapAdvancedProfileEndpoints", programContent, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Mediator_wiring_matches_selected_mediator()
    {
        var mediator = ArchitectureTestContext.GetOption("AegisMediator");
        var dispatching = Path.Combine(ArchitectureTestContext.BuildingBlocksProjectRoot, "Cqrs", "DispatchingServiceCollectionExtensions.cs");
        var content = File.ReadAllText(dispatching);

        if (mediator == "mediatr")
        {
            Assert.Contains("services.AddMediatR", content, StringComparison.Ordinal);
            Assert.Contains("MediatRCommandDispatcher", content, StringComparison.Ordinal);
            Assert.Contains("MediatRQueryDispatcher", content, StringComparison.Ordinal);
            Assert.Contains("ISender sender", content, StringComparison.Ordinal);
            Assert.DoesNotContain("RegisterCoreHandlers(services", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ServiceProviderCommandDispatcher", content, StringComparison.Ordinal);
            return;
        }

        Assert.Contains("RegisterCoreHandlers(services", content, StringComparison.Ordinal);
        Assert.Contains("ServiceProviderCommandDispatcher", content, StringComparison.Ordinal);
        Assert.Contains("ServiceProviderQueryDispatcher", content, StringComparison.Ordinal);
        Assert.DoesNotContain("services.AddMediatR", content, StringComparison.Ordinal);
        Assert.DoesNotContain("MediatRCommandDispatcher", content, StringComparison.Ordinal);
        Assert.DoesNotContain("MediatRQueryDispatcher", content, StringComparison.Ordinal);
    }
}
