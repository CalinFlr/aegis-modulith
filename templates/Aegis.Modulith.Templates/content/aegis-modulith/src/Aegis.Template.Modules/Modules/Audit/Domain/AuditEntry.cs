namespace Aegis.Template.Modules.Modules.Audit.Domain;

public sealed class AuditEntry
{
    private AuditEntry()
    {
    }

    public AuditEntry(Guid id, string stream, string action)
    {
        Id = id;
        Stream = stream;
        Action = action;
        RecordedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Stream { get; private set; } = string.Empty;

    public string Action { get; private set; } = string.Empty;

    public DateTimeOffset RecordedAtUtc { get; private set; }
}
