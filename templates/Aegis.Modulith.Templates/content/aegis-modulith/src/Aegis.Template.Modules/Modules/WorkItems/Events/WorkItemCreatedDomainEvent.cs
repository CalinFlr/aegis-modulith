using Aegis.Template.BuildingBlocks.Domain;

namespace Aegis.Template.Modules.Modules.WorkItems.Events;

public sealed record WorkItemCreatedDomainEvent(Guid WorkItemId, DateTimeOffset OccurredAtUtc)
    : DomainEvent(OccurredAtUtc);
