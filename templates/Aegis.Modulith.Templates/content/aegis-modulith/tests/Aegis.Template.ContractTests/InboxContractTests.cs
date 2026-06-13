using System.Text.Json;
using Aegis.Template.Api.Pro.Infrastructure.Inbox;
using Aegis.Template.BuildingBlocks.Events;
#if (sample == "taskhub")
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.Tasks.Contracts.TaskCreatedIntegrationEvent;
#else
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.WorkItems.Contracts.WorkItemCreatedIntegrationEvent;
#endif

namespace Aegis.Template.ContractTests;

public sealed class InboxContractTests
{
    [Fact]
    public void Inbox_message_payload_contract_is_serializable_and_keeps_message_identity()
    {
        var integrationEvent = CreateSampleEvent();
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();
        var payload = JsonSerializer.Serialize(integrationEvent);
        var message = new InboxMessage(integrationEvent.Id, $"inbox:{integrationEvent.Id:N}", messageType, payload, DateTimeOffset.UtcNow);

        var deserialized = JsonSerializer.Deserialize<SampleIntegrationEvent>(message.Payload);

        Assert.NotNull(deserialized);
        Assert.Equal(integrationEvent.Id, deserialized.Id);
        Assert.Equal(message.MessageId, deserialized.Id);
        Assert.Equal($"inbox:{integrationEvent.Id:N}", message.IdempotencyKey);
        Assert.Equal(messageType, message.MessageType);
    }

    [Fact]
    public void Inbox_contract_tests_do_not_require_broker_dependencies_or_exactly_once_claims()
    {
        var forbiddenBrokerMarkers = new[]
        {
            "MassTransit",
            "RabbitMQ",
            "Confluent.Kafka",
            "Azure.Messaging.ServiceBus",
            "NServiceBus"
        };
        var forbiddenGuaranteeMarkers = new[]
        {
            "exactly-once broker",
            "broker-level exactly-once",
            "exactly once delivery"
        };
        var failures = new List<string>();

        foreach (var sourceFile in ContractTestContext.SourceFilesUnder(ContractTestContext.SourceRoot))
        {
            var content = File.ReadAllText(sourceFile);
            foreach (var marker in forbiddenBrokerMarkers.Concat(forbiddenGuaranteeMarkers))
            {
                if (content.Contains(marker, StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{ContractTestContext.Relative(sourceFile)} contains forbidden marker {marker}.");
                }
            }
        }

        Assert.Empty(failures);
    }

    private static SampleIntegrationEvent CreateSampleEvent()
    {
#if (sample == "taskhub")
        return new SampleIntegrationEvent(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            new DateTimeOffset(2026, 06, 09, 12, 0, 0, TimeSpan.Zero));
#else
        return new SampleIntegrationEvent(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            new DateTimeOffset(2026, 06, 09, 12, 0, 0, TimeSpan.Zero));
#endif
    }
}
