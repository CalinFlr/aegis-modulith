using System.Text.Json;

namespace Aegis.Template.ArchitectureTests;

public sealed class ModuleManifestTests
{
    [Fact]
    public void Every_module_has_a_valid_manifest_with_boundary_rules()
    {
        var moduleFolders = ArchitectureTestContext.ModuleFolders;

        Assert.NotEmpty(moduleFolders);

        foreach (var moduleFolder in moduleFolders)
        {
            var manifestPath = Path.Combine(moduleFolder, "module.json");
            Assert.True(File.Exists(manifestPath), $"{Path.GetFileName(moduleFolder)} is missing module.json.");

            using var manifest = ReadManifest(moduleFolder);
            var root = manifest.RootElement;

            foreach (var property in new[] { "name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules" })
            {
                Assert.True(root.TryGetProperty(property, out _), $"{manifestPath} missing {property}.");
            }

            Assert.Equal(Path.GetFileName(moduleFolder), GetRequiredString(root, "name", manifestPath));
            Assert.Equal("business-module", GetRequiredString(root, "type", manifestPath));
            Assert.False(string.IsNullOrWhiteSpace(GetRequiredString(root, "schema", manifestPath)), $"{manifestPath} schema must not be empty.");

            var rules = root.GetProperty("rules");
            Assert.False(rules.GetProperty("allowCrossModuleDatabaseAccess").GetBoolean());
            Assert.False(rules.GetProperty("allowInfrastructureReferences").GetBoolean());
        }
    }

    [Fact]
    public void Public_contracts_listed_in_manifests_exist_under_contracts_folder()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            using var manifest = ReadManifest(moduleFolder);
            var moduleName = Path.GetFileName(moduleFolder);
            var contractsFolder = Path.Combine(moduleFolder, "Contracts");

            foreach (var contractName in GetStringArray(manifest.RootElement, "publicContracts", moduleFolder))
            {
                var contractPath = Path.Combine(contractsFolder, $"{contractName}.cs");
                if (!File.Exists(contractPath))
                {
                    failures.Add($"{moduleName} lists public contract {contractName}, but {ArchitectureTestContext.Relative(contractPath)} does not exist.");
                }
            }
        }

        Assert.Empty(failures);
    }

    internal static JsonDocument ReadManifest(string moduleFolder)
    {
        return JsonDocument.Parse(File.ReadAllText(Path.Combine(moduleFolder, "module.json")));
    }

    internal static IReadOnlyList<string> GetStringArray(JsonElement root, string propertyName, string manifestContext)
    {
        var property = GetRequiredProperty(root, propertyName, manifestContext);
        Assert.Equal(JsonValueKind.Array, property.ValueKind);

        return property.EnumerateArray()
            .Select(item =>
            {
                Assert.Equal(JsonValueKind.String, item.ValueKind);
                return item.GetString() ?? string.Empty;
            })
            .ToArray();
    }

    private static string GetRequiredString(JsonElement root, string propertyName, string manifestContext)
    {
        var property = GetRequiredProperty(root, propertyName, manifestContext);
        Assert.Equal(JsonValueKind.String, property.ValueKind);
        return property.GetString() ?? string.Empty;
    }

    private static JsonElement GetRequiredProperty(JsonElement root, string propertyName, string manifestContext)
    {
        Assert.True(root.TryGetProperty(propertyName, out var property), $"{manifestContext} missing {propertyName}.");
        return property;
    }
}
