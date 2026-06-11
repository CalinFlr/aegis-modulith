namespace Aegis.Template.Api.Pro.Outbox;

public sealed record OutboxMessage(Guid Id, string Type, string Payload, DateTimeOffset OccurredAtUtc, DateTimeOffset? ProcessedAtUtc);
