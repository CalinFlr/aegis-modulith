using AegisBuildingBlocksNamespace.Events;

namespace AegisItemRootNamespace.Modules.AegisEventModule.Contracts.IntegrationEvents;

public sealed record Aegis.EventIntegrationEvent(
    Guid Id,
    Guid AggregateId,
    DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
