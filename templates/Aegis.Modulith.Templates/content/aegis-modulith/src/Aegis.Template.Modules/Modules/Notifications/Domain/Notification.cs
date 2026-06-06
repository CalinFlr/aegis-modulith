namespace Aegis.Template.Modules.Modules.Notifications.Domain;

public sealed class Notification
{
    private Notification()
    {
    }

    public Notification(Guid id, string recipient, string message)
    {
        Id = id;
        Recipient = recipient;
        Message = message;
        QueuedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Recipient { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public DateTimeOffset QueuedAtUtc { get; private set; }
}
