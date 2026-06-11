namespace Aegis.Template.Modules.Modules.WorkItems.Features.CreateWorkItem;

public sealed record CreateWorkItemResponse(Guid Id, string Title, DateTimeOffset CreatedAtUtc);
