using AegisBuildingBlocksNamespace.Domain;

namespace AegisItemRootNamespace.Modules.AegisEventModule.Domain.Events;

public sealed record Aegis.EventDomainEvent(Guid AggregateId, DateTimeOffset OccurredAtUtc)
    : DomainEvent(OccurredAtUtc);
