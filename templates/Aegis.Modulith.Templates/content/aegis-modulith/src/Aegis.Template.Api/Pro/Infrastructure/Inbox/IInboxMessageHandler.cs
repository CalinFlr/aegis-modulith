namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public interface IInboxMessageHandler
{
    string MessageType { get; }

    Task HandleAsync(InboxMessage message, CancellationToken cancellationToken);
}
