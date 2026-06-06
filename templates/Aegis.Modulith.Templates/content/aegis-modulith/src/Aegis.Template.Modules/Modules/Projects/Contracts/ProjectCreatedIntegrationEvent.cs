using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Projects.Contracts;

public sealed record ProjectCreatedIntegrationEvent(Guid Id, Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
