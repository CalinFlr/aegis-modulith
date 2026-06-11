using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.Projects.Features.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IQuery<GetProjectByIdResponse?>
#if (mediator == "mediatr")
    , MediatR.IRequest<GetProjectByIdResponse?>
#endif
;
