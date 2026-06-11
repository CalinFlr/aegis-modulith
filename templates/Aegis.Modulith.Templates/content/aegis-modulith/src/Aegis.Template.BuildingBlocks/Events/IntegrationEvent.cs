namespace Aegis.Template.BuildingBlocks.Events;

public abstract record IntegrationEvent(Guid Id, DateTimeOffset OccurredAtUtc);
