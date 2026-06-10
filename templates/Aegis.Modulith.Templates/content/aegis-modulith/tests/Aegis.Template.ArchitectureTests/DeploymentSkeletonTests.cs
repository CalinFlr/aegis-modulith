namespace Aegis.Template.ArchitectureTests;

public sealed class DeploymentSkeletonTests
{
    [Fact]
    public void Deployment_skeleton_matches_selected_profile()
    {
        var profile = ArchitectureTestContext.GetOption("AegisProfile");
        var dockerfile = Path.Combine(ArchitectureTestContext.SolutionRoot, "Dockerfile");
        var dockerignore = Path.Combine(ArchitectureTestContext.SolutionRoot, ".dockerignore");
        var compose = Path.Combine(ArchitectureTestContext.SolutionRoot, "docker-compose.yml");
        var envExample = Path.Combine(ArchitectureTestContext.SolutionRoot, ".env.example");
        var productionSettings = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "appsettings.Production.json");
        var deploymentDocs = Path.Combine(ArchitectureTestContext.SolutionRoot, "docs", "deployment.md");
        var workflow = Path.Combine(ArchitectureTestContext.SolutionRoot, ".github", "workflows", "ci.yml");

        Assert.True(File.Exists(deploymentDocs), "Generated outputs should document deployment behavior.");

        if (profile == "core")
        {
            Assert.False(File.Exists(dockerfile), "core profile must not include the pro/advanced Dockerfile.");
            Assert.False(File.Exists(dockerignore), "core profile must not include the pro/advanced .dockerignore.");
            Assert.False(File.Exists(compose), "core profile must not include local compose deployment assets.");
            Assert.False(File.Exists(envExample), "core profile must not include pro/advanced environment examples.");
            Assert.False(File.Exists(productionSettings), "core profile must not include pro/advanced production config examples.");
            Assert.Contains("core profile keeps deployment scaffolding lightweight", File.ReadAllText(deploymentDocs), StringComparison.OrdinalIgnoreCase);
            if (File.Exists(workflow))
            {
                Assert.DoesNotContain("docker build", File.ReadAllText(workflow), StringComparison.Ordinal);
                Assert.DoesNotContain("deployment-placeholder", File.ReadAllText(workflow), StringComparison.Ordinal);
            }

            return;
        }

        Assert.True(File.Exists(dockerfile), $"{profile} profile must include Dockerfile.");
        Assert.True(File.Exists(dockerignore), $"{profile} profile must include .dockerignore.");
        Assert.True(File.Exists(compose), $"{profile} profile must include local docker-compose.yml.");
        Assert.True(File.Exists(envExample), $"{profile} profile must include .env.example.");
        Assert.True(File.Exists(productionSettings), $"{profile} profile must include appsettings.Production.json.");

        var dockerfileContent = File.ReadAllText(dockerfile);
        Assert.Contains($"src/{ArchitectureTestContext.ProjectPrefix}.Api/{ArchitectureTestContext.ProjectPrefix}.Api.csproj", dockerfileContent.Replace('\\', '/'), StringComparison.Ordinal);
        Assert.Contains($"{ArchitectureTestContext.ProjectPrefix}.Api.dll", dockerfileContent, StringComparison.Ordinal);
        Assert.Contains("HEALTHCHECK", dockerfileContent, StringComparison.Ordinal);
        Assert.Contains("http://localhost:8080/health", dockerfileContent, StringComparison.Ordinal);

        var envContent = File.ReadAllText(envExample);
        Assert.Contains("ASPNETCORE_ENVIRONMENT=Production", envContent, StringComparison.Ordinal);
        Assert.Contains("ConnectionStrings__Postgres=", envContent, StringComparison.Ordinal);
        Assert.Contains("Authentication__Jwt__Issuer=", envContent, StringComparison.Ordinal);
        Assert.Contains("Authentication__Jwt__Audience=", envContent, StringComparison.Ordinal);
        Assert.Contains("Authentication__Jwt__SigningKey=", envContent, StringComparison.Ordinal);
        Assert.Contains("OTEL_EXPORTER_OTLP_ENDPOINT=", envContent, StringComparison.Ordinal);
        Assert.Contains("Do not commit real", envContent, StringComparison.Ordinal);

        var productionSettingsContent = File.ReadAllText(productionSettings);
        Assert.Contains("\"Postgres\": \"\"", productionSettingsContent, StringComparison.Ordinal);
        Assert.Contains("\"SigningKey\": \"\"", productionSettingsContent, StringComparison.Ordinal);
        Assert.Contains("\"EnableBackgroundProcessor\": false", productionSettingsContent, StringComparison.Ordinal);

        var composeContent = File.ReadAllText(compose);
        Assert.Contains("postgres:17-alpine", composeContent, StringComparison.Ordinal);
        Assert.Contains("Set POSTGRES_PASSWORD", composeContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Password=postgres", composeContent, StringComparison.OrdinalIgnoreCase);

        var docsContent = File.ReadAllText(deploymentDocs);
        Assert.Contains("not full production infrastructure", docsContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("No registry, organization, repository, cloud provider, or deployment target is hardcoded", docsContent, StringComparison.Ordinal);
        Assert.Contains("/health", docsContent, StringComparison.Ordinal);
        Assert.Contains("No collector is required by default", docsContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Deployment_files_do_not_contain_hardcoded_real_secrets_or_provider_targets()
    {
        var profile = ArchitectureTestContext.GetOption("AegisProfile");
        if (profile == "core")
        {
            return;
        }

        var deploymentFiles = new[]
        {
            Path.Combine(ArchitectureTestContext.SolutionRoot, "Dockerfile"),
            Path.Combine(ArchitectureTestContext.SolutionRoot, ".env.example"),
            Path.Combine(ArchitectureTestContext.SolutionRoot, "docker-compose.yml"),
            Path.Combine(ArchitectureTestContext.ApiProjectRoot, "appsettings.Production.json"),
            Path.Combine(ArchitectureTestContext.SolutionRoot, ".github", "workflows", "ci.yml")
        };

        foreach (var file in deploymentFiles.Where(File.Exists))
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("super-secret", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("password123", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("ghcr.io/", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("docker.io/", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("azurecr.io", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("amazonaws.com", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("gcr.io", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Production_projects_do_not_reference_deployment_scripts_or_workflow_files()
    {
        var projectFiles = Directory
            .EnumerateFiles(ArchitectureTestContext.SourceRoot, "*.csproj", SearchOption.AllDirectories)
            .ToArray();

        foreach (var projectFile in projectFiles)
        {
            var content = File.ReadAllText(projectFile);
            Assert.DoesNotContain("Dockerfile", content, StringComparison.Ordinal);
            Assert.DoesNotContain("docker-compose", content, StringComparison.Ordinal);
            Assert.DoesNotContain(".github", content, StringComparison.Ordinal);
            Assert.DoesNotContain(".env.example", content, StringComparison.Ordinal);
        }
    }
}
