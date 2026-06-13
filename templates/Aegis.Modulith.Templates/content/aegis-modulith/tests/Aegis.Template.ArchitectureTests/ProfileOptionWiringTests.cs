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
            Assert.DoesNotContain("UseAuthentication", File.ReadAllText(program), StringComparison.Ordinal);
            Assert.DoesNotContain("UseAuthorization", File.ReadAllText(program), StringComparison.Ordinal);
            return;
        }

        Assert.True(File.Exists(appHostProject), $"{profile} profile must include AppHost.");
        Assert.True(File.Exists(serviceDefaultsProject), $"{profile} profile must include ServiceDefaults.");
        Assert.True(File.Exists(dockerfile), $"{profile} profile must include Dockerfile.");
        Assert.Contains("ServiceDefaults", File.ReadAllText(apiProject), StringComparison.Ordinal);
        Assert.Contains("AppHost", File.ReadAllText(solution), StringComparison.Ordinal);

        var programContent = File.ReadAllText(program);
        Assert.Contains("builder.AddServiceDefaults();", programContent, StringComparison.Ordinal);
        Assert.Contains("builder.Services.AddProProfileServices(builder.Configuration);", programContent, StringComparison.Ordinal);
        Assert.Contains("app.UseRateLimiter();", programContent, StringComparison.Ordinal);
        Assert.Contains("app.UseAuthentication();", programContent, StringComparison.Ordinal);
        Assert.Contains("app.UseAuthorization();", programContent, StringComparison.Ordinal);
        Assert.Contains("app.MapProProfileEndpoints();", programContent, StringComparison.Ordinal);
        Assert.True(
            programContent.IndexOf("app.UseAuthentication();", StringComparison.Ordinal) <
            programContent.IndexOf("app.UseAuthorization();", StringComparison.Ordinal),
            "UseAuthentication must run before UseAuthorization.");

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

    [Fact]
    public void Production_projects_do_not_reference_fake_authentication_test_infrastructure()
    {
        var productionFiles = Directory
            .EnumerateFiles(ArchitectureTestContext.SourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

        foreach (var file in productionFiles)
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("FakeAuthentication", content, StringComparison.Ordinal);
            Assert.DoesNotContain("Aegis.Test", content, StringComparison.Ordinal);
            Assert.DoesNotContain("X-Test-", content, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Authorization_policy_usage_uses_named_constants()
    {
        var sourceFiles = Directory
            .EnumerateFiles(ArchitectureTestContext.SourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var file in sourceFiles)
        {
            Assert.DoesNotContain("RequireAuthorization(\"", File.ReadAllText(file), StringComparison.Ordinal);
        }

        if (ArchitectureTestContext.GetOption("AegisProfile") != "core")
        {
            Assert.Contains(sourceFiles, file => File.ReadAllText(file).Contains("AegisAuthorizationPolicies.", StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Auth_configuration_does_not_contain_hardcoded_production_secrets()
    {
        var sourceFiles = Directory
            .EnumerateFiles(ArchitectureTestContext.SourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var file in sourceFiles)
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("SigningKey = \"", content, StringComparison.Ordinal);
            Assert.DoesNotContain("super-secret", content, StringComparison.OrdinalIgnoreCase);
        }

        var appSettings = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "appsettings.json");
        if (File.Exists(appSettings))
        {
            var content = File.ReadAllText(appSettings);
            Assert.DoesNotContain("\"SigningKey\": \"", content, StringComparison.Ordinal);
            Assert.DoesNotContain("super-secret", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Jwt_options_require_sufficient_signing_key_length()
    {
        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            return;
        }

        var jwtOptions = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "Pro", "Auth", "AegisJwtOptions.cs");
        var content = File.ReadAllText(jwtOptions);

        Assert.Contains("MinimumSigningKeyBytes = 32", content, StringComparison.Ordinal);
        Assert.Contains("Encoding.UTF8.GetByteCount", content, StringComparison.Ordinal);
        Assert.Contains("HasSufficientSigningKey", content, StringComparison.Ordinal);
    }
}
