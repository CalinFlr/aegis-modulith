using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.Projects.Features.CreateProject;

public sealed record CreateProjectCommand(string Name) : ICommand<CreateProjectResponse>
#if AEGIS_MEDIATR
    , MediatR.IRequest<CreateProjectResponse>
#endif
;
