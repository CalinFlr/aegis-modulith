namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class InboxProcessor(
    IInboxStore inboxStore,
    IEnumerable<IInboxMessageHandler> handlers,
    ILogger<InboxProcessor> logger)
{
    private readonly Dictionary<string, IInboxMessageHandler> handlersByType = handlers
        .ToDictionary(handler => handler.MessageType, StringComparer.Ordinal);

    public async Task<int> ProcessPendingAsync(int maxMessages = 20, CancellationToken cancellationToken = default)
    {
        var pending = await inboxStore.GetPendingAsync(maxMessages, cancellationToken);
        var processed = 0;

        foreach (var pendingMessage in pending)
        {
            var message = await inboxStore.TryBeginProcessingAsync(pendingMessage.MessageId, cancellationToken);
            if (message is null)
            {
                continue;
            }

            if (!handlersByType.TryGetValue(message.MessageType, out var handler))
            {
                await inboxStore.MarkFailedAsync(
                    message.MessageId,
                    $"No inbox handler is registered for message type '{message.MessageType}'.",
                    cancellationToken);
                continue;
            }

            try
            {
                await handler.HandleAsync(message, cancellationToken);
                await inboxStore.MarkProcessedAsync(message.MessageId, cancellationToken);
                processed++;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Inbox message {MessageId} failed while dispatching.", message.MessageId);
                await inboxStore.MarkFailedAsync(message.MessageId, exception.Message, cancellationToken);
            }
        }

        return processed;
    }
}
