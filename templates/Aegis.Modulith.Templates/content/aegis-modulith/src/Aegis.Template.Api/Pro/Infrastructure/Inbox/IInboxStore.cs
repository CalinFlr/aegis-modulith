namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public interface IInboxStore
{
    Task<InboxAcceptResult> AcceptAsync(
        Guid messageId,
        string messageType,
        string payload,
        CancellationToken cancellationToken = default);

    Task<bool> IsDuplicateAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InboxMessage>> GetPendingAsync(int maxMessages, CancellationToken cancellationToken = default);

    Task<InboxMessage?> TryBeginProcessingAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task MarkProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task MarkFailedAsync(Guid messageId, string failureReason, CancellationToken cancellationToken = default);
}
