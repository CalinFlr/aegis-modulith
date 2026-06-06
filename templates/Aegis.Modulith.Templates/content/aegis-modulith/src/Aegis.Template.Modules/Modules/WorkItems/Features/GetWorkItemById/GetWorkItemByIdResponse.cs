namespace Aegis.Template.Modules.Modules.WorkItems.Features.GetWorkItemById;

public sealed record GetWorkItemByIdResponse(Guid Id, string Title, DateTimeOffset CreatedAtUtc);
