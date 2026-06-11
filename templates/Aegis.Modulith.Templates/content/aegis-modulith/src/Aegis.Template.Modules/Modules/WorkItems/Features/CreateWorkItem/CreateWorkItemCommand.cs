using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.WorkItems.Features.CreateWorkItem;

public sealed record CreateWorkItemCommand(string Title) : ICommand<CreateWorkItemResponse>
#if AEGIS_MEDIATR
    , MediatR.IRequest<CreateWorkItemResponse>
#endif
;
