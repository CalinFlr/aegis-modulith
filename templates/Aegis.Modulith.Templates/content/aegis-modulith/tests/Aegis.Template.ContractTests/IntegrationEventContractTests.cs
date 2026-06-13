using System.Reflection;
using System.Text.Json;
using Aegis.Template.Api.Pro.Infrastructure.Inbox;
using Aegis.Template.BuildingBlocks.Domain;
using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.ContractTests;

public sealed class IntegrationEventContractTests
{
    [Fact]
    public void Integration_events_have_type_and_version_metadata_and_live_under_contracts()
    {
        var integrationEventTypes = IntegrationEventTypes();
        Assert.NotEmpty(integrationEventTypes);

        foreach (var eventType in integrationEventTypes)
        {
            var attribute = eventType.GetCustomAttribute<IntegrationEventContractAttribute>();
            Assert.NotNull(attribute);
            Assert.False(string.IsNullOrWhiteSpace(attribute.Type));
            Assert.True(attribute.Version >= 1);
            Assert.Contains(".Contracts", eventType.Namespace ?? string.Empty, StringComparison.Ordinal);
            Assert.DoesNotContain(".Domain", eventType.Namespace ?? string.Empty, StringComparison.Ordinal);
        }

        foreach (var contractFile in IntegrationContractFiles())
        {
            Assert.Contains($"{Path.DirectorySeparatorChar}Contracts{Path.DirectorySeparatorChar}", contractFile, StringComparison.Ordinal);
            Assert.Contains("IntegrationEventContract", File.ReadAllText(contractFile), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Integration_event_contracts_round_trip_with_system_text_json()
    {
        foreach (var eventType in IntegrationEventTypes())
        {
            var original = CreateSampleEvent(eventType);
            var json = JsonSerializer.Serialize(original, eventType);
            var deserialized = JsonSerializer.Deserialize(json, eventType);

            Assert.NotNull(deserialized);
            foreach (var property in eventType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                Assert.Equal(property.GetValue(original), property.GetValue(deserialized));
            }
        }
    }

    [Fact]
    public void Domain_events_and_integration_events_remain_distinct()
    {
        var domainEventTypes = ContractTestContext.ModuleTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true })
            .Where(type => typeof(DomainEvent).IsAssignableFrom(type))
            .ToArray();

        Assert.NotEmpty(domainEventTypes);
        foreach (var domainEventType in domainEventTypes)
        {
            Assert.False(typeof(IntegrationEvent).IsAssignableFrom(domainEventType));
            Assert.DoesNotContain(".Contracts", domainEventType.Namespace ?? string.Empty, StringComparison.Ordinal);
            Assert.Null(domainEventType.GetCustomAttribute<IntegrationEventContractAttribute>());
        }
    }

    [Fact]
    public void Inbox_handler_consumes_integration_contract_metadata_not_domain_entities()
    {
        var handler = new SampleIntegrationEventInboxHandler(Microsoft.Extensions.Logging.Abstractions.NullLogger<SampleIntegrationEventInboxHandler>.Instance);
        var messageType = handler.MessageType;

        Assert.Contains(messageType, IntegrationEventTypes().Select(IntegrationEventContractMetadata.TypeName));

        var handlerSource = File.ReadAllText(Path.Combine(
            ContractTestContext.ApiProjectRoot,
            "Pro",
            "Infrastructure",
            "Inbox",
            "SampleIntegrationEventInboxHandler.cs"));

        Assert.Contains("IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>()", handlerSource, StringComparison.Ordinal);
        Assert.Contains(".Contracts", handlerSource, StringComparison.Ordinal);
        Assert.DoesNotContain(".Domain", handlerSource, StringComparison.Ordinal);
    }

    private static IReadOnlyList<Type> IntegrationEventTypes()
    {
        return ContractTestContext.ModuleTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true })
            .Where(type => typeof(IntegrationEvent).IsAssignableFrom(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
    }

    private static IEnumerable<string> IntegrationContractFiles()
    {
        return ContractTestContext.ModuleFolders
            .Select(moduleFolder => Path.Combine(moduleFolder, "Contracts"))
            .SelectMany(contractsFolder => ContractTestContext.SourceFilesUnder(contractsFolder))
            .Where(path => Path.GetFileName(path).EndsWith("IntegrationEvent.cs", StringComparison.Ordinal));
    }

    private static object CreateSampleEvent(Type eventType)
    {
        var constructor = eventType.GetConstructors().Single();
        var arguments = constructor.GetParameters()
            .Select(parameter => SampleValue(parameter.ParameterType, parameter.Name ?? string.Empty))
            .ToArray();

        return constructor.Invoke(arguments);
    }

    private static object SampleValue(Type type, string name)
    {
        if (type == typeof(Guid))
        {
            return Guid.Parse(name.Contains("project", StringComparison.OrdinalIgnoreCase)
                ? "22222222-2222-2222-2222-222222222222"
                : "11111111-1111-1111-1111-111111111111");
        }

        if (type == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(2026, 06, 09, 12, 0, 0, TimeSpan.Zero);
        }

        if (type == typeof(string))
        {
            return $"sample-{name}";
        }

        if (type == typeof(int))
        {
            return 1;
        }

        throw new NotSupportedException($"No sample value is defined for {type.FullName}.");
    }
}
