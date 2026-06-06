namespace Aegis.Template.Modules.Modules.Tasks.Features.ListTasks;

public sealed record ListTasksResponse(Guid Id, Guid ProjectId, string Title, DateTimeOffset CreatedAtUtc);
