using System.Text.Json;
using Aegis.Template.Modules;

namespace Aegis.Template.ArchitectureTests;

public sealed class ModuleManifestTests
{
    [Fact]
    public void Every_module_has_a_valid_manifest()
    {
        var modulesRoot = FindModulesRoot();
        var moduleFolders = Directory.GetDirectories(modulesRoot);

        Assert.NotEmpty(moduleFolders);

        foreach (var moduleFolder in moduleFolders)
        {
            var manifestPath = Path.Combine(moduleFolder, "module.json");
            Assert.True(File.Exists(manifestPath), $"{Path.GetFileName(moduleFolder)} is missing module.json.");

            using var manifest = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var root = manifest.RootElement;

            foreach (var property in new[] { "name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules" })
            {
                Assert.True(root.TryGetProperty(property, out _), $"{manifestPath} missing {property}.");
            }

            var rules = root.GetProperty("rules");
            Assert.False(rules.GetProperty("allowCrossModuleDatabaseAccess").GetBoolean());
            Assert.False(rules.GetProperty("allowInfrastructureReferences").GetBoolean());
        }
    }

    private static string FindModulesRoot()
    {
        var modulesAssemblyName = typeof(ModulesAssemblyMarker).Assembly.GetName().Name
            ?? throw new InvalidOperationException("Modules assembly name is unavailable.");

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "src", modulesAssemblyName, "Modules");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException($"Could not find src/{modulesAssemblyName}/Modules.");
    }
}
