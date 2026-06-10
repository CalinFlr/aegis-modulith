namespace Aegis.Template.ArchitectureTests;

public sealed class ContractBoundaryTests
{
    [Fact]
    public void Contract_test_project_matches_selected_profile()
    {
        var contractTestProject = Path.Combine(
            ArchitectureTestContext.SolutionRoot,
            "tests",
            $"{ArchitectureTestContext.ProjectPrefix}.ContractTests",
            $"{ArchitectureTestContext.ProjectPrefix}.ContractTests.csproj");
        var solution = File.ReadAllText(Path.Combine(ArchitectureTestContext.SolutionRoot, $"{ArchitectureTestContext.ProjectPrefix}.sln"));

        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            Assert.False(File.Exists(contractTestProject), "Core profile should not include the pro/advanced contract test project.");
            Assert.DoesNotContain("ContractTests", solution, StringComparison.Ordinal);
            return;
        }

        Assert.True(File.Exists(contractTestProject), "Pro and advanced profiles should include the generated contract test project.");
        Assert.Contains($"{ArchitectureTestContext.ProjectPrefix}.ContractTests", solution, StringComparison.Ordinal);
    }

    [Fact]
    public void Production_projects_do_not_reference_contract_tests()
    {
        var failures = new List<string>();
        foreach (var projectFile in Directory.GetFiles(ArchitectureTestContext.SourceRoot, "*.csproj", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(projectFile);
            if (content.Contains("ContractTests", StringComparison.Ordinal))
            {
                failures.Add($"{ArchitectureTestContext.Relative(projectFile)} references generated contract tests.");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Integration_contracts_do_not_depend_on_infrastructure()
    {
        var forbidden = new[]
        {
            ".Infrastructure",
            "Microsoft.EntityFrameworkCore",
            "Npgsql",
            "DbContext",
            "InboxMessage"
        };
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var contractsFolder = Path.Combine(moduleFolder, "Contracts");
            foreach (var sourceFile in ArchitectureTestContext.SourceFilesUnder(contractsFolder))
            {
                var content = File.ReadAllText(sourceFile);
                foreach (var marker in forbidden)
                {
                    if (content.Contains(marker, StringComparison.Ordinal))
                    {
                        failures.Add($"{ArchitectureTestContext.Relative(sourceFile)} contains infrastructure marker {marker}.");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Production_api_contract_does_not_reference_fake_auth_test_infrastructure()
    {
        var forbidden = new[] { "Aegis.Test", "FakeAuthentication", "X-Test-User-Id", "X-Test-Permissions" };
        var failures = new List<string>();

        foreach (var sourceFile in ArchitectureTestContext.SourceFilesUnder(ArchitectureTestContext.ApiProjectRoot))
        {
            var content = File.ReadAllText(sourceFile);
            foreach (var marker in forbidden)
            {
                if (content.Contains(marker, StringComparison.Ordinal))
                {
                    failures.Add($"{ArchitectureTestContext.Relative(sourceFile)} contains test-only auth marker {marker}.");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Performance_smoke_test_project_matches_selected_profile()
    {
        var performanceSmokeProject = Path.Combine(
            ArchitectureTestContext.SolutionRoot,
            "tests",
            $"{ArchitectureTestContext.ProjectPrefix}.PerformanceSmokeTests",
            $"{ArchitectureTestContext.ProjectPrefix}.PerformanceSmokeTests.csproj");
        var solution = File.ReadAllText(Path.Combine(ArchitectureTestContext.SolutionRoot, $"{ArchitectureTestContext.ProjectPrefix}.sln"));

        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            Assert.False(File.Exists(performanceSmokeProject), "Core profile should not include the pro/advanced performance smoke test project.");
            Assert.DoesNotContain("PerformanceSmokeTests", solution, StringComparison.Ordinal);
            return;
        }

        Assert.True(File.Exists(performanceSmokeProject), "Pro and advanced profiles should include the generated performance smoke test project.");
        Assert.Contains($"{ArchitectureTestContext.ProjectPrefix}.PerformanceSmokeTests", solution, StringComparison.Ordinal);
    }

    [Fact]
    public void Production_projects_do_not_reference_performance_smoke_tests()
    {
        var failures = new List<string>();
        foreach (var projectFile in Directory.GetFiles(ArchitectureTestContext.SourceRoot, "*.csproj", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(projectFile);
            if (content.Contains("PerformanceSmokeTests", StringComparison.Ordinal))
            {
                failures.Add($"{ArchitectureTestContext.Relative(projectFile)} references generated performance smoke tests.");
            }
        }

        Assert.Empty(failures);
    }
}
