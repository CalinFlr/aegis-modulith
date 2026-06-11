using Aegis.Template.BuildingBlocks.Domain;

namespace Aegis.Template.Modules.Modules.Tasks.Events;

public sealed record TaskCreatedDomainEvent(Guid TaskId, Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : DomainEvent(OccurredAtUtc);
