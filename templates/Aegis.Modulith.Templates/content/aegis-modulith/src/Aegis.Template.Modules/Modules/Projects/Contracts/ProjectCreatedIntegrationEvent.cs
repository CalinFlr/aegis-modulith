using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Projects.Contracts;

[IntegrationEventContract("projects.created", 1)]
public sealed record ProjectCreatedIntegrationEvent(Guid Id, Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
