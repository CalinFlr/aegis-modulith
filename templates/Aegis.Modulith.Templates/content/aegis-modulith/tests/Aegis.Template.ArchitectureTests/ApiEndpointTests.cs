namespace Aegis.Template.ArchitectureTests;

public sealed class ApiEndpointTests
{
    [Fact]
    public void Module_endpoint_mappings_do_not_perform_persistence_directly()
    {
        foreach (var endpointBody in ModuleEndpointBodies())
        {
            Assert.DoesNotContain("SaveChanges(", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("SaveChangesAsync(", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain("AddDbContext", endpointBody.Body, StringComparison.Ordinal);
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
        foreach (var endpointBody in ModuleEndpointBodies())
        {
            Assert.Contains("Results.", endpointBody.Body, StringComparison.Ordinal);
            Assert.DoesNotContain(".Domain", endpointBody.Body, StringComparison.Ordinal);
        }
    }

    private static IReadOnlyList<EndpointBody> ModuleEndpointBodies()
    {
        return ArchitectureTestContext.ModuleFolders
            .Select(moduleFolder => Path.Combine(moduleFolder, $"{Path.GetFileName(moduleFolder)}Module.cs"))
            .Where(File.Exists)
            .Select(file =>
            {
                var source = File.ReadAllText(file);
                return new EndpointBody(ArchitectureTestContext.Relative(file), ArchitectureTestContext.ExtractMethodBody(source, "void MapEndpoints"));
            })
            .ToArray();
    }

    private sealed record EndpointBody(string RelativePath, string Body);
}

