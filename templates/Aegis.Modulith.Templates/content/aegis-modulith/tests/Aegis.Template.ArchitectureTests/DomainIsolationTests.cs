using Aegis.Template.BuildingBlocks.Domain;

namespace Aegis.Template.ArchitectureTests;

public sealed class DomainIsolationTests
{
    [Fact]
    public void Domain_source_files_do_not_depend_on_web_or_persistence_infrastructure()
    {
        var domainFiles = DomainModelSourceFiles().ToArray();

        Assert.NotEmpty(domainFiles);

        var forbiddenMarkers = new[]
        {
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore",
            "Npgsql",
            ".Infrastructure"
        };
        var failures = new List<string>();

        foreach (var file in domainFiles)
        {
            var content = File.ReadAllText(file);
            foreach (var marker in forbiddenMarkers.Where(marker => content.Contains(marker, StringComparison.Ordinal)))
            {
                failures.Add($"{ArchitectureTestContext.Relative(file)} must not depend on {marker}.");
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Domain_source_files_do_not_reference_other_module_domain_namespaces()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var moduleName = Path.GetFileName(moduleFolder);
            var domainSource = string.Join(Environment.NewLine, DomainModelSourceFiles(moduleFolder).Select(File.ReadAllText));
            foreach (var otherModuleName in ArchitectureTestContext.ModuleNames.Where(name => name != moduleName))
            {
                var forbiddenNamespace = $".Modules.{otherModuleName}.Domain";
                if (domainSource.Contains(forbiddenNamespace, StringComparison.Ordinal))
                {
                    failures.Add($"{moduleName} domain references another module domain namespace: {forbiddenNamespace}");
                }
            }
        }

        Assert.Empty(failures);
    }

    private static IEnumerable<string> DomainModelSourceFiles()
    {
        return ArchitectureTestContext.ModuleFolders.SelectMany(DomainModelSourceFiles);
    }

    private static IEnumerable<string> DomainModelSourceFiles(string moduleFolder)
    {
        var domainFolder = Path.Combine(moduleFolder, "Domain");
        var domainFiles = Directory.Exists(domainFolder)
            ? ArchitectureTestContext.SourceFilesUnder(domainFolder)
            : [];

        var domainEventFiles = Directory.GetFiles(moduleFolder, "*DomainEvent.cs", SearchOption.AllDirectories);

        return domainFiles.Concat(domainEventFiles).Distinct(StringComparer.Ordinal);
    }

    [Fact]
    public void Domain_events_are_module_owned_and_follow_the_domain_event_abstraction()
    {
        var domainEventTypes = ArchitectureTestContext.ModuleTypes()
            .Where(type => type != typeof(DomainEvent))
            .Where(type => type.Name.EndsWith("DomainEvent", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(domainEventTypes);

        foreach (var domainEventType in domainEventTypes)
        {
            Assert.True(
                typeof(DomainEvent).IsAssignableFrom(domainEventType),
                $"{domainEventType.FullName} must derive from DomainEvent.");
            Assert.Contains(".Modules.", domainEventType.Namespace ?? string.Empty, StringComparison.Ordinal);
            Assert.DoesNotContain(".Contracts", domainEventType.Namespace ?? string.Empty, StringComparison.Ordinal);
        }

        var domainEventFiles = Directory.GetFiles(ArchitectureTestContext.ModulesRoot, "*DomainEvent.cs", SearchOption.AllDirectories);
        Assert.NotEmpty(domainEventFiles);

        foreach (var file in domainEventFiles)
        {
            var relative = ArchitectureTestContext.Relative(file);
            Assert.Contains("/Events/", relative, StringComparison.Ordinal);
            Assert.Contains(": DomainEvent", File.ReadAllText(file), StringComparison.Ordinal);
        }
    }
}

