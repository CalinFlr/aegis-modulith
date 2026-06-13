namespace Aegis.Template.ArchitectureTests;

public sealed class ApiEndpointTests
{
    [Fact]
    public void Module_endpoint_mappings_do_not_perform_persistence_directly()
    {
        foreach (var endpointBody in ModuleEndpointBodies())
        {
            foreach (var dbContextTypeName in ModuleDbContextTypeNames())
            {
                Assert.DoesNotContain(dbContextTypeName, endpointBody.Body, StringComparison.Ordinal);
            }

            Assert.DoesNotContain("SaveChanges(", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("SaveChangesAsync(", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("AddDbContext", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("DbSet<", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain(".Set<", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("UseNpgsql", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain(".Infrastructure", endpointBody.Body, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Module_endpoint_mappings_delegate_feature_requests_to_dispatchers()
    {
        foreach (var endpointBody in ModuleEndpointBodies())
        {
            if (endpointBody.Body.Contains("Command", StringComparison.Ordinal))
            {
                Assert.Contains("ICommandDispatcher", endpointBody.Body, StringComparison.Ordinal);
            }

            if (endpointBody.Body.Contains("Query", StringComparison.Ordinal))
            {
                Assert.Contains("IQueryDispatcher", endpointBody.Body, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void Module_endpoint_mappings_use_http_result_mapping_without_domain_namespaces()
    {
        foreach (var endpointBody in ModuleEndpointBodies().Where(body => body.MapsHttpVerbDirectly))
        {
            Assert.Contains("Results.", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain(".Domain", endpointBody.Body, StringComparison.Ordinal);
        }
    }

    private static IReadOnlyList<EndpointBody> ModuleEndpointBodies()
    {
        var moduleEndpointBodies = ArchitectureTestContext.ModuleFolders
            .Select(moduleFolder => Path.Combine(moduleFolder, $"{Path.GetFileName(moduleFolder)}Module.cs"))
            .Where(File.Exists)
            .Select(file =>
            {
                var source = File.ReadAllText(file);
                var body = ArchitectureTestContext.ExtractMethodBody(source, "void MapEndpoints");
                return new EndpointBody(ArchitectureTestContext.Relative(file), body, MapsHttpVerbDirectly(body));
            })
            .ToArray();

        var featureEndpointBodies = ArchitectureTestContext.ModuleFolders
            .Select(moduleFolder => Path.Combine(moduleFolder, "Features"))
            .Where(Directory.Exists)
            .SelectMany(featuresFolder => Directory.GetFiles(featuresFolder, "*Endpoint.cs", SearchOption.AllDirectories))
            .Select(file =>
            {
                var body = File.ReadAllText(file);
                return new EndpointBody(ArchitectureTestContext.Relative(file), body, MapsHttpVerbDirectly(body));
            })
            .ToArray();

        return moduleEndpointBodies.Concat(featureEndpointBodies).ToArray();
    }

    private static IReadOnlyList<string> ModuleDbContextTypeNames()
    {
        return Directory.GetFiles(ArchitectureTestContext.ModulesRoot, "*DbContext.cs", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool MapsHttpVerbDirectly(string body)
    {
        return body.Contains(".MapGet(", StringComparison.Ordinal) ||
               body.Contains(".MapPost(", StringComparison.Ordinal) ||
               body.Contains(".MapPut(", StringComparison.Ordinal) ||
               body.Contains(".MapPatch(", StringComparison.Ordinal) ||
               body.Contains(".MapDelete(", StringComparison.Ordinal);
    }

    private sealed record EndpointBody(string RelativePath, string Body, bool MapsHttpVerbDirectly);
}

