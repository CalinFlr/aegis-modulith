namespace Aegis.Template.Modules.Modules.Projects.Features.GetProjectById;

public sealed record GetProjectByIdResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);
