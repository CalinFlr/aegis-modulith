using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Notifications.Contracts;

[IntegrationEventContract("notifications.queued", 1)]
public sealed record NotificationQueuedIntegrationEvent(Guid Id, Guid NotificationId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
