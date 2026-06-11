using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.WorkItems.Features.CreateWorkItem;

public sealed record CreateWorkItemCommand(string Title) : ICommand<CreateWorkItemResponse>
#if (mediator == "mediatr")
    , MediatR.IRequest<CreateWorkItemResponse>
#endif
;
