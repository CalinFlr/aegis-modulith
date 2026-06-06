using Aegis.Template.BuildingBlocks.Domain;

namespace Aegis.Template.Modules.Modules.Projects.Events;

public sealed record ProjectCreatedDomainEvent(Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : DomainEvent(OccurredAtUtc);
