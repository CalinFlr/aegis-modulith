using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules;

namespace Aegis.Template.ArchitectureTests;

public sealed class ModuleBoundaryTests
{
    [Fact]
    public void Modules_do_not_reference_another_modules_infrastructure_folder()
    {
        var sourceRoot = FindSourceRoot();
        var projectFiles = Directory.GetFiles(sourceRoot, "*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            var content = File.ReadAllText(projectFile);
            Assert.DoesNotContain("Infrastructure", content, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Generated_code_does_not_use_generic_repositories()
    {
        var sourceRoot = FindSourceRoot();
        var sourceFiles = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories);

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
        var handlerTypes = typeof(ModulesAssemblyMarker)
            .Assembly
            .GetTypes()
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

    private static string FindSourceRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "src");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find src folder.");
    }
}
