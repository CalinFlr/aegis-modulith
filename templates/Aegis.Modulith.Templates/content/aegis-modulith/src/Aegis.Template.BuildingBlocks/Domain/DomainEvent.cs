namespace Aegis.Template.BuildingBlocks.Domain;

public abstract record DomainEvent(DateTimeOffset OccurredAtUtc);
