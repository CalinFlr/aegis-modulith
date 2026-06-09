using System.Reflection;
using System.Xml.Linq;
using Aegis.Template.Modules;

namespace Aegis.Template.ContractTests;

internal static class ContractTestContext
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

    public static string ApiProjectRoot => Path.Combine(SourceRoot, $"{ProjectPrefix}.Api");

    public static IReadOnlyList<string> ModuleFolders => Directory.GetDirectories(ModulesRoot)
        .OrderBy(path => path, StringComparer.Ordinal)
        .ToArray();

    public static IEnumerable<Type> ModuleTypes()
    {
        return typeof(ModulesAssemblyMarker).Assembly.GetTypes();
    }

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
