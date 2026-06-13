#if (sample == "taskhub")
using Aegis.Template.Modules.Modules.Tasks.Contracts;
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.Tasks.Contracts.TaskCreatedIntegrationEvent;
#else
using Aegis.Template.Modules.Modules.WorkItems.Contracts;
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.WorkItems.Contracts.WorkItemCreatedIntegrationEvent;
#endif
using System.Text.Json;

namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class SampleIntegrationEventInboxHandler(ILogger<SampleIntegrationEventInboxHandler> logger) : IInboxMessageHandler
{
    public string MessageType => typeof(SampleIntegrationEvent).FullName ?? nameof(SampleIntegrationEvent);

    public Task HandleAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        var integrationEvent = JsonSerializer.Deserialize<SampleIntegrationEvent>(message.Payload);
        if (integrationEvent is null)
        {
            throw new InvalidOperationException($"Inbox payload for message {message.MessageId} could not be deserialized.");
        }

        logger.LogInformation(
            "Handled inbox integration event {MessageId} of type {MessageType}.",
            integrationEvent.Id,
            MessageType);

        return Task.CompletedTask;
    }
}
