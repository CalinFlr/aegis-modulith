using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Tasks.Contracts;

public sealed record TaskCreatedIntegrationEvent(Guid Id, Guid TaskId, Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
