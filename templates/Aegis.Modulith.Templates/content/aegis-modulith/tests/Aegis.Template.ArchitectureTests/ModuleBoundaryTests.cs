using Aegis.Template.BuildingBlocks.Cqrs;
using System.Xml.Linq;

namespace Aegis.Template.ArchitectureTests;

public sealed class ModuleBoundaryTests
{
    [Fact]
    public void Project_references_do_not_point_to_infrastructure_projects()
    {
        var failures = new List<string>();
        var projectFiles = Directory.GetFiles(ArchitectureTestContext.SourceRoot, "*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            var project = XDocument.Load(projectFile);
            var references = project
                .Descendants("ProjectReference")
                .Select(element => element.Attribute("Include")?.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Cast<string>();

            foreach (var reference in references)
            {
                var normalized = reference.Replace('\\', '/');
                if (normalized.Contains("/Infrastructure/", StringComparison.Ordinal) ||
                    normalized.Contains(".Infrastructure", StringComparison.Ordinal))
                {
                    failures.Add($"{ArchitectureTestContext.Relative(projectFile)} references an Infrastructure project: {reference}");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Modules_do_not_reference_another_modules_infrastructure_namespace()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var moduleName = Path.GetFileName(moduleFolder);
            var moduleSource = string.Join(Environment.NewLine, ArchitectureTestContext.SourceFilesUnder(moduleFolder).Select(File.ReadAllText));

            foreach (var otherModuleName in ArchitectureTestContext.ModuleNames.Where(name => name != moduleName))
            {
                var forbiddenNamespace = $".Modules.{otherModuleName}.Infrastructure";
                if (moduleSource.Contains(forbiddenNamespace, StringComparison.Ordinal))
                {
                    failures.Add($"{moduleName} references another module Infrastructure namespace: {forbiddenNamespace}");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Cross_module_contract_references_are_declared_in_module_manifests()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var moduleName = Path.GetFileName(moduleFolder);
            using var manifest = ModuleManifestTests.ReadManifest(moduleFolder);
            var dependencies = ModuleManifestTests.GetStringArray(manifest.RootElement, "dependencies", moduleFolder);
            var moduleSource = string.Join(Environment.NewLine, ArchitectureTestContext.SourceFilesUnder(moduleFolder).Select(File.ReadAllText));

            foreach (var dependency in dependencies)
            {
                if (!dependency.EndsWith(".Contracts", StringComparison.Ordinal))
                {
                    failures.Add($"{moduleName} manifest dependency must target Contracts only by default: {dependency}");
                    continue;
                }

                var dependencyModuleName = dependency[..^".Contracts".Length];
                if (!ArchitectureTestContext.ModuleNames.Contains(dependencyModuleName, StringComparer.Ordinal))
                {
                    failures.Add($"{moduleName} manifest dependency references unknown module: {dependency}");
                }
            }

            foreach (var otherModuleName in ArchitectureTestContext.ModuleNames.Where(name => name != moduleName))
            {
                var contractsNamespace = $".Modules.{otherModuleName}.Contracts";
                var dependencyName = $"{otherModuleName}.Contracts";

                if (moduleSource.Contains(contractsNamespace, StringComparison.Ordinal) &&
                    !dependencies.Contains(dependencyName, StringComparer.Ordinal))
                {
                    failures.Add($"{moduleName} references {contractsNamespace} but module.json does not declare {dependencyName}.");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Generated_code_does_not_use_generic_repositories()
    {
        var sourceFiles = Directory.GetFiles(ArchitectureTestContext.SourceRoot, "*.cs", SearchOption.AllDirectories);

        foreach (var sourceFile in sourceFiles)
        {
            var content = File.ReadAllText(sourceFile);
            Assert.DoesNotContain("Repository<", content, StringComparison.Ordinal);
            Assert.DoesNotContain("IRepository", content, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Cqrs_handlers_are_discoverable()
    {
        var handlerTypes = ArchitectureTestContext.ModuleTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => type.GetInterfaces().Any(IsCqrsHandler))
            .ToArray();

        Assert.NotEmpty(handlerTypes);
    }

    private static bool IsCqrsHandler(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(ICommandHandler<,>) ||
               genericTypeDefinition == typeof(IQueryHandler<,>);
    }
}
