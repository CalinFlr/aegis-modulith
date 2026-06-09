using System.Reflection;
using System.Xml.Linq;
using Aegis.Template.Modules;

namespace Aegis.Template.ArchitectureTests;

internal static class ArchitectureTestContext
{
    private static readonly Lazy<string> SolutionRootValue = new(FindSolutionRoot);
    private static readonly Lazy<XDocument> DirectoryBuildPropsValue = new(() => XDocument.Load(Path.Combine(SolutionRoot, "Directory.Build.props")));

    public static string SolutionRoot => SolutionRootValue.Value;

    public static string SourceRoot => Path.Combine(SolutionRoot, "src");

    public static string ModulesAssemblyName => typeof(ModulesAssemblyMarker).Assembly.GetName().Name
        ?? throw new InvalidOperationException("Modules assembly name is unavailable.");

    public static string ProjectPrefix => ModulesAssemblyName.EndsWith(".Modules", StringComparison.Ordinal)
        ? ModulesAssemblyName[..^".Modules".Length]
        : ModulesAssemblyName;

    public static string ModulesProjectRoot => Path.Combine(SourceRoot, ModulesAssemblyName);

    public static string ModulesRoot => Path.Combine(ModulesProjectRoot, "Modules");

    public static string BuildingBlocksProjectRoot => Path.Combine(SourceRoot, $"{ProjectPrefix}.BuildingBlocks");

    public static string ApiProjectRoot => Path.Combine(SourceRoot, $"{ProjectPrefix}.Api");

    public static IReadOnlyList<string> ModuleFolders => Directory.GetDirectories(ModulesRoot)
        .OrderBy(path => path, StringComparer.Ordinal)
        .ToArray();

    public static IReadOnlyList<string> ModuleNames => ModuleFolders
        .Select(path => Path.GetFileName(path))
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Cast<string>()
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToArray();

    public static string GetOption(string name)
    {
        var value = DirectoryBuildPropsValue.Value
            .Root?
            .Elements("PropertyGroup")
            .Elements(name)
            .FirstOrDefault()?
            .Value;

        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"Directory.Build.props is missing {name}.")
            : value;
    }

    public static IEnumerable<Type> ModuleTypes()
    {
        return typeof(ModulesAssemblyMarker).Assembly.GetTypes();
    }

    public static IEnumerable<string> SourceFilesUnder(string root)
    {
        return Directory.Exists(root)
            ? Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.Ordinal)
            : [];
    }

    public static string Relative(string path)
    {
        return Path.GetRelativePath(SolutionRoot, path).Replace('\\', '/');
    }

    public static IReadOnlyList<Type> GetOpenGenericInterfaces(Type type, Type openGenericType)
    {
        return type.GetInterfaces()
            .Where(candidate => candidate.IsGenericType)
            .Where(candidate => candidate.GetGenericTypeDefinition() == openGenericType)
            .ToArray();
    }

    public static string ExtractMethodBody(string source, string signatureFragment)
    {
        var signatureIndex = source.IndexOf(signatureFragment, StringComparison.Ordinal);
        if (signatureIndex < 0)
        {
            throw new InvalidOperationException($"Could not find method signature fragment '{signatureFragment}'.");
        }

        var openBrace = source.IndexOf('{', signatureIndex);
        if (openBrace < 0)
        {
            throw new InvalidOperationException($"Could not find method body for '{signatureFragment}'.");
        }

        var depth = 0;
        for (var index = openBrace; index < source.Length; index++)
        {
            depth += source[index] switch
            {
                '{' => 1,
                '}' => -1,
                _ => 0
            };

            if (depth == 0)
            {
                return source[openBrace..(index + 1)];
            }
        }

        throw new InvalidOperationException($"Could not parse method body for '{signatureFragment}'.");
    }

    private static string FindSolutionRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Directory.Build.props")) &&
                Directory.Exists(Path.Combine(current.FullName, "src")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find generated solution root.");
    }
}
