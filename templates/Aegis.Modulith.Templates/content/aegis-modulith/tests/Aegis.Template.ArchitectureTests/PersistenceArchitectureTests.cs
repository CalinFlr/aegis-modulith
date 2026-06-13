using System.Text.RegularExpressions;

namespace Aegis.Template.ArchitectureTests;

public sealed class PersistenceArchitectureTests
{
    [Fact]
    public void Each_module_has_one_module_scoped_dbcontext()
    {
        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var moduleName = Path.GetFileName(moduleFolder);
            var infrastructureFolder = Path.Combine(moduleFolder, "Infrastructure");
            Assert.True(Directory.Exists(infrastructureFolder), $"{moduleName} must have an Infrastructure folder.");

            var dbContextFiles = Directory.GetFiles(infrastructureFolder, "*DbContext.cs", SearchOption.TopDirectoryOnly);
            Assert.Single(dbContextFiles);

            var content = File.ReadAllText(dbContextFiles[0]);
            Assert.Contains($"namespace {ArchitectureTestContext.ModulesAssemblyName}.Modules.{moduleName}.Infrastructure;", content, StringComparison.Ordinal);
            Assert.Contains(": DbContext", content, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Dbcontext_default_schemas_match_module_manifests()
    {
        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            using var manifest = ModuleManifestTests.ReadManifest(moduleFolder);
            var schema = manifest.RootElement.GetProperty("schema").GetString()
                ?? throw new InvalidOperationException($"{moduleFolder} manifest schema is missing.");
            var dbContextFile = Directory.GetFiles(Path.Combine(moduleFolder, "Infrastructure"), "*DbContext.cs", SearchOption.TopDirectoryOnly).Single();
            var content = File.ReadAllText(dbContextFile);

            Assert.Contains("HasDefaultSchema", content, StringComparison.Ordinal);
            var literalSchema = Regex.IsMatch(content, $@"HasDefaultSchema\s*\(\s*""{Regex.Escape(schema)}""\s*\)");
            var constantSchema = Regex.IsMatch(content, $@"const\s+string\s+Schema\s*=\s*""{Regex.Escape(schema)}""") &&
                Regex.IsMatch(content, @"HasDefaultSchema\s*\(\s*Schema\s*\)");
            Assert.True(literalSchema || constantSchema, $"{ArchitectureTestContext.Relative(dbContextFile)} must set the default schema to {schema}.");
        }
    }

    [Fact]
    public void Generated_dbcontexts_do_not_configure_foreign_keys_by_default()
    {
        var forbiddenMarkers = new[]
        {
            ".HasForeignKey(",
            "HasForeignKey<",
            ".HasOne(",
            ".HasMany(",
            ".WithOne(",
            ".WithMany(",
            "[ForeignKey",
            "[InverseProperty",
            "System.ComponentModel.DataAnnotations.Schema"
        };

        foreach (var sourceFile in ArchitectureTestContext.SourceFilesUnder(ArchitectureTestContext.ModulesRoot))
        {
            var content = File.ReadAllText(sourceFile);
            foreach (var marker in forbiddenMarkers)
            {
                Assert.DoesNotContain(marker, content, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void Persistence_and_domain_sources_do_not_reference_other_module_domain_or_infrastructure_namespaces()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var moduleName = Path.GetFileName(moduleFolder);
            var foldersToCheck = new[]
            {
                Path.Combine(moduleFolder, "Domain"),
                Path.Combine(moduleFolder, "Infrastructure")
            };
            var source = string.Join(Environment.NewLine, foldersToCheck
                .Where(Directory.Exists)
                .SelectMany(ArchitectureTestContext.SourceFilesUnder)
                .Select(File.ReadAllText));

            foreach (var otherModuleName in ArchitectureTestContext.ModuleNames.Where(name => name != moduleName))
            {
                foreach (var forbiddenNamespace in new[]
                         {
                             $".Modules.{otherModuleName}.Domain",
                             $".Modules.{otherModuleName}.Infrastructure"
                         })
                {
                    if (source.Contains(forbiddenNamespace, StringComparison.Ordinal))
                    {
                        failures.Add($"{moduleName} persistence/domain source references {forbiddenNamespace}.");
                    }
                }
            }
        }

        Assert.Empty(failures);
    }
}

