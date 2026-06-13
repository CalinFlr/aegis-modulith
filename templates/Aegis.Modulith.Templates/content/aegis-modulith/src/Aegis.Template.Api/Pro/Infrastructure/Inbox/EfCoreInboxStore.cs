using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class EfCoreInboxStore(AegisInboxDbContext dbContext, ILogger<EfCoreInboxStore> logger) : IInboxStore
{
    private static readonly TimeSpan ProcessingLease = TimeSpan.FromMinutes(5);

    public async Task<InboxAcceptResult> AcceptAsync(
        Guid messageId,
        string messageType,
        string payload,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.InboxMessages
            .SingleOrDefaultAsync(message => message.MessageId == messageId, cancellationToken);

        if (existing is not null)
        {
            return existing.IsProcessed ? InboxAcceptResult.AlreadyProcessed : InboxAcceptResult.Duplicate;
        }

        var message = new InboxMessage(
            messageId,
            messageId.ToString("N"),
            messageType,
            payload,
            DateTimeOffset.UtcNow);

        dbContext.InboxMessages.Add(message);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Accepted inbox message {MessageId} of type {MessageType}.", messageId, messageType);
            return InboxAcceptResult.Accepted;
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            logger.LogInformation("Inbox message {MessageId} was already accepted by another request.", messageId);
            return InboxAcceptResult.Duplicate;
        }
    }

    public Task<bool> IsDuplicateAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return dbContext.InboxMessages.AnyAsync(message => message.MessageId == messageId, cancellationToken);
    }

    public async Task<IReadOnlyList<InboxMessage>> GetPendingAsync(int maxMessages, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        return await dbContext.InboxMessages
            .Where(message =>
                message.Status == InboxMessageStatus.Pending ||
                message.Status == InboxMessageStatus.Failed ||
                (message.Status == InboxMessageStatus.Processing &&
                    message.LockedUntilUtc != null &&
                    message.LockedUntilUtc <= now))
            .OrderBy(message => message.ReceivedAtUtc)
            .Take(maxMessages)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<InboxMessage?> TryBeginProcessingAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.InboxMessages
            .SingleOrDefaultAsync(candidate => candidate.MessageId == messageId, cancellationToken);

        if (message is null || message.Status == InboxMessageStatus.Processed)
        {
            return null;
        }

        if (message.Status == InboxMessageStatus.Processing &&
            message.LockedUntilUtc is not null &&
            message.LockedUntilUtc > DateTimeOffset.UtcNow)
        {
            return null;
        }

        message.MarkProcessing(Guid.NewGuid(), DateTimeOffset.UtcNow.Add(ProcessingLease));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return message;
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogInformation("Inbox message {MessageId} was claimed by another processor.", messageId);
            return null;
        }
    }

    public async Task MarkProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.InboxMessages
            .SingleAsync(candidate => candidate.MessageId == messageId, cancellationToken);

        message.MarkProcessed(DateTimeOffset.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Processed inbox message {MessageId}.", messageId);
    }

    public async Task MarkFailedAsync(Guid messageId, string failureReason, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.InboxMessages
            .SingleAsync(candidate => candidate.MessageId == messageId, cancellationToken);

        message.MarkFailed(failureReason);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogWarning("Inbox message {MessageId} failed: {FailureReason}", messageId, failureReason);
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}
