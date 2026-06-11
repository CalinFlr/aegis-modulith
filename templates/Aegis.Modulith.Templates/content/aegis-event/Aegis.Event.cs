namespace AegisEventModule.Events;

public sealed record Aegis.Event(Guid Id, DateTimeOffset OccurredAtUtc, string Scope)
{
    public static Aegis.Event Create() => new(Guid.NewGuid(), DateTimeOffset.UtcNow, "AegisEventScope");
}
