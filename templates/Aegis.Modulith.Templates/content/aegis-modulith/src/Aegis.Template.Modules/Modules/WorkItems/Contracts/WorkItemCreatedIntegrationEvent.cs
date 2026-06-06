using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.WorkItems.Contracts;

public sealed record WorkItemCreatedIntegrationEvent(Guid Id, Guid WorkItemId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
