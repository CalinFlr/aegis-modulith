using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Tasks.Contracts;

[IntegrationEventContract("tasks.created", 1)]
public sealed record TaskCreatedIntegrationEvent(Guid Id, Guid TaskId, Guid ProjectId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
