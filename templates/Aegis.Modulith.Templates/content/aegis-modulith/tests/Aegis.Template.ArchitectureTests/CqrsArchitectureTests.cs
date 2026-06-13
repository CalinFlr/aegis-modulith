using System.Reflection;
using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.ArchitectureTests;

public sealed class CqrsArchitectureTests
{
    [Fact]
    public void Commands_and_queries_follow_generated_abstractions()
    {
        var commandTypes = FeatureTypesEndingWith("Command");
        var queryTypes = FeatureTypesEndingWith("Query");

        Assert.NotEmpty(commandTypes);
        Assert.NotEmpty(queryTypes);

        foreach (var commandType in commandTypes)
        {
            Assert.NotEmpty(ArchitectureTestContext.GetOpenGenericInterfaces(commandType, typeof(ICommand<>)));
            AssertMediatRRequestShapeIfEnabled(commandType);
        }

        foreach (var queryType in queryTypes)
        {
            Assert.NotEmpty(ArchitectureTestContext.GetOpenGenericInterfaces(queryType, typeof(IQuery<>)));
            AssertMediatRRequestShapeIfEnabled(queryType);
        }
    }

    [Fact]
    public void Command_and_query_handlers_are_bound_to_their_requests()
    {
        var commandTypes = FeatureTypesEndingWith("Command");
        var queryTypes = FeatureTypesEndingWith("Query");
        var commandHandlerBindings = HandlerBindings(typeof(ICommandHandler<,>));
        var queryHandlerBindings = HandlerBindings(typeof(IQueryHandler<,>));

        Assert.NotEmpty(commandHandlerBindings);
        Assert.NotEmpty(queryHandlerBindings);

        foreach (var commandType in commandTypes)
        {
            var responseType = ArchitectureTestContext.GetOpenGenericInterfaces(commandType, typeof(ICommand<>))
                .Single()
                .GetGenericArguments()[0];
            AssertResponseTypeDoesNotExposeDomainOrInfrastructure(commandType, responseType);
            Assert.Contains(commandHandlerBindings, binding => binding.RequestType == commandType && binding.ResponseType == responseType);
        }

        foreach (var queryType in queryTypes)
        {
            var responseType = ArchitectureTestContext.GetOpenGenericInterfaces(queryType, typeof(IQuery<>))
                .Single()
                .GetGenericArguments()[0];
            AssertResponseTypeDoesNotExposeDomainOrInfrastructure(queryType, responseType);
            Assert.Contains(queryHandlerBindings, binding => binding.RequestType == queryType && binding.ResponseType == responseType);
        }

        if (ArchitectureTestContext.GetOption("AegisMediator") == "mediatr")
        {
            foreach (var binding in commandHandlerBindings.Concat(queryHandlerBindings))
            {
                Assert.True(
                    ImplementsMediatRInterface(binding.HandlerType, "MediatR.IRequestHandler`2"),
                    $"{binding.HandlerType.FullName} must implement MediatR.IRequestHandler in MediatR mode.");
            }
        }
    }

    [Fact]
    public void Query_handlers_do_not_mutate_state_and_use_no_tracking_for_ef_queries()
    {
        var queryHandlerFiles = ArchitectureTestContext.SourceFilesUnder(ArchitectureTestContext.ModulesRoot)
            .Where(file => File.ReadAllText(file).Contains("IQueryHandler<", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(queryHandlerFiles);

        foreach (var file in queryHandlerFiles)
        {
            var content = File.ReadAllText(file);
            Assert.DoesNotContain("SaveChanges(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("SaveChangesAsync(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteUpdate(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteUpdateAsync(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteDelete(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteDeleteAsync(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteSql(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteSqlRaw(", content, StringComparison.Ordinal);
            Assert.DoesNotContain("ExecuteSqlInterpolated(", content, StringComparison.Ordinal);

            if (content.Contains("DbContext", StringComparison.Ordinal) ||
                content.Contains("Microsoft.EntityFrameworkCore", StringComparison.Ordinal))
            {
                Assert.True(UsesNoTrackingQuery(content), $"{ArchitectureTestContext.Relative(file)} must use a no-tracking EF query API.");
            }
        }
    }

    [Fact]
    public void Command_and_query_slice_files_live_under_feature_folders()
    {
        var sliceFiles = ArchitectureTestContext.SourceFilesUnder(ArchitectureTestContext.ModulesRoot)
            .Where(IsSliceSourceFile)
            .ToArray();

        Assert.NotEmpty(sliceFiles);

        foreach (var file in sliceFiles)
        {
            var relative = ArchitectureTestContext.Relative(file);
            var parts = relative.Split('/');
            var featuresIndex = Array.IndexOf(parts, "Features");
            Assert.True(featuresIndex >= 0 && featuresIndex + 1 < parts.Length, $"{relative} must be under Features/<SliceName>.");

            var sliceName = parts[featuresIndex + 1];
            var fileName = Path.GetFileNameWithoutExtension(file);
            Assert.StartsWith(sliceName, fileName, StringComparison.Ordinal);
        }
    }

    private static void AssertResponseTypeDoesNotExposeDomainOrInfrastructure(Type requestType, Type responseType)
    {
        foreach (var type in FlattenResponseTypes(responseType))
        {
            var fullName = type.FullName ?? type.Name;
            Assert.DoesNotContain(".Domain.", fullName, StringComparison.Ordinal);
            Assert.DoesNotContain(".Infrastructure.", fullName, StringComparison.Ordinal);
            Assert.False(
                type.Namespace?.Contains(".Domain", StringComparison.Ordinal) == true,
                $"{requestType.FullName} must not expose domain type {fullName} as its CQRS response.");
            Assert.False(
                type.Namespace?.Contains(".Infrastructure", StringComparison.Ordinal) == true,
                $"{requestType.FullName} must not expose infrastructure type {fullName} as its CQRS response.");
        }
    }

    private static IEnumerable<Type> FlattenResponseTypes(Type type)
    {
        return FlattenResponseTypes(type, []);
    }

    private static IEnumerable<Type> FlattenResponseTypes(Type type, HashSet<Type> visited)
    {
        var effectiveType = Nullable.GetUnderlyingType(type) ?? type;
        if (!visited.Add(effectiveType))
        {
            yield break;
        }

        yield return effectiveType;

        if (effectiveType == typeof(string) ||
            effectiveType.IsPrimitive ||
            effectiveType.IsEnum ||
            effectiveType.Namespace?.StartsWith("System", StringComparison.Ordinal) == true)
        {
            yield break;
        }

        foreach (var argument in effectiveType.GetGenericArguments())
        {
            foreach (var nested in FlattenResponseTypes(argument, visited))
            {
                yield return nested;
            }
        }

        foreach (var memberType in PublicMemberTypes(effectiveType))
        {
            foreach (var nested in FlattenResponseTypes(memberType, visited))
            {
                yield return nested;
            }
        }
    }

    private static IReadOnlyList<Type> FeatureTypesEndingWith(string suffix)
    {
        return ArchitectureTestContext.ModuleTypes()
            .Where(type => type.Namespace?.Contains(".Features.", StringComparison.Ordinal) == true)
            .Where(type => type.Name.EndsWith(suffix, StringComparison.Ordinal))
            .ToArray();
    }

    private static IEnumerable<Type> PublicMemberTypes(Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            yield return property.PropertyType;
        }

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            yield return field.FieldType;
        }

        foreach (var parameter in type
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(constructor => constructor.GetParameters()))
        {
            yield return parameter.ParameterType;
        }
    }

    private static IReadOnlyList<HandlerBinding> HandlerBindings(Type handlerOpenGenericType)
    {
        return ArchitectureTestContext.ModuleTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .SelectMany(type => ArchitectureTestContext.GetOpenGenericInterfaces(type, handlerOpenGenericType)
                .Select(handlerInterface =>
                {
                    var arguments = handlerInterface.GetGenericArguments();
                    return new HandlerBinding(type, arguments[0], arguments[1]);
                }))
            .ToArray();
    }

    private static void AssertMediatRRequestShapeIfEnabled(Type requestType)
    {
        if (ArchitectureTestContext.GetOption("AegisMediator") != "mediatr")
        {
            Assert.False(
                ImplementsMediatRInterface(requestType, "MediatR.IRequest`1"),
                $"{requestType.FullName} must not implement MediatR.IRequest in core mediator mode.");
            return;
        }

        Assert.True(
            ImplementsMediatRInterface(requestType, "MediatR.IRequest`1"),
            $"{requestType.FullName} must implement MediatR.IRequest in MediatR mode.");
    }

    private static bool ImplementsMediatRInterface(Type type, string openGenericFullName)
    {
        return type.GetInterfaces()
            .Where(candidate => candidate.IsGenericType)
            .Select(candidate => candidate.GetGenericTypeDefinition().FullName)
            .Any(fullName => fullName == openGenericFullName);
    }

    private static bool UsesNoTrackingQuery(string content)
    {
        return content.Contains(".AsNoTracking()", StringComparison.Ordinal) ||
               content.Contains(".AsNoTrackingWithIdentityResolution()", StringComparison.Ordinal) ||
               content.Contains("QueryTrackingBehavior.NoTracking", StringComparison.Ordinal) ||
               content.Contains("UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)", StringComparison.Ordinal);
    }

    private static bool IsSliceSourceFile(string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        return name.EndsWith("Command", StringComparison.Ordinal) ||
               name.EndsWith("Query", StringComparison.Ordinal) ||
               (name.EndsWith("Handler", StringComparison.Ordinal) && IsCqrsHandlerSource(file)) ||
               name.EndsWith("Response", StringComparison.Ordinal);
    }

    private static bool IsCqrsHandlerSource(string file)
    {
        var content = File.ReadAllText(file);
        return content.Contains("ICommandHandler<", StringComparison.Ordinal) ||
               content.Contains("IQueryHandler<", StringComparison.Ordinal);
    }

    private sealed record HandlerBinding(Type HandlerType, Type RequestType, Type ResponseType);
}

