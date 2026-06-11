using Aegis.Template.BuildingBlocks.Events;

namespace Aegis.Template.Modules.Modules.Notifications.Contracts;

public sealed record NotificationQueuedIntegrationEvent(Guid Id, Guid NotificationId, DateTimeOffset OccurredAtUtc)
    : IntegrationEvent(Id, OccurredAtUtc);
