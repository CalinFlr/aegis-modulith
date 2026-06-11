using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.WorkItems.Features.GetWorkItemById;

public sealed record GetWorkItemByIdQuery(Guid Id) : IQuery<GetWorkItemByIdResponse?>
#if (mediator == "mediatr")
    , MediatR.IRequest<GetWorkItemByIdResponse?>
#endif
;
