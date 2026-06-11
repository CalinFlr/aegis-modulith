namespace Aegis.Template.Modules.Modules.Tasks.Features.CreateTask;

public sealed record CreateTaskResponse(Guid Id, Guid ProjectId, string Title, DateTimeOffset CreatedAtUtc);
