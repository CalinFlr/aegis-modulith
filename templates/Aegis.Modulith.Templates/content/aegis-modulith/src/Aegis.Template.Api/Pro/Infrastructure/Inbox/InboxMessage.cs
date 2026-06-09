namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    public InboxMessage(Guid messageId, string idempotencyKey, string messageType, string payload, DateTimeOffset receivedAtUtc)
    {
        Id = Guid.NewGuid();
        MessageId = messageId;
        IdempotencyKey = idempotencyKey;
        MessageType = messageType;
        Payload = payload;
        Status = InboxMessageStatus.Pending;
        ReceivedAtUtc = receivedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid MessageId { get; private set; }

    public string IdempotencyKey { get; private set; } = string.Empty;

    public string MessageType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public InboxMessageStatus Status { get; private set; }

    public DateTimeOffset ReceivedAtUtc { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public string? FailureReason { get; private set; }

    public int AttemptCount { get; private set; }

    public Guid? LockToken { get; private set; }

    public DateTimeOffset? LockedUntilUtc { get; private set; }

    public bool IsProcessed => Status == InboxMessageStatus.Processed;

    public void MarkProcessing(Guid lockToken, DateTimeOffset lockedUntilUtc)
    {
        Status = InboxMessageStatus.Processing;
        AttemptCount++;
        LockToken = lockToken;
        LockedUntilUtc = lockedUntilUtc;
        FailureReason = null;
    }

    public void MarkProcessed(DateTimeOffset processedAtUtc)
    {
        Status = InboxMessageStatus.Processed;
        ProcessedAtUtc = processedAtUtc;
        FailureReason = null;
        LockToken = null;
        LockedUntilUtc = null;
    }

    public void MarkFailed(string failureReason)
    {
        Status = InboxMessageStatus.Failed;
        FailureReason = failureReason;
        LockToken = null;
        LockedUntilUtc = null;
    }
}
