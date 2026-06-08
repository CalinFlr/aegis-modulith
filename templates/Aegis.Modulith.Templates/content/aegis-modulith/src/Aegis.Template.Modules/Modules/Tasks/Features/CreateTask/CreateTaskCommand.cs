using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.Tasks.Features.CreateTask;

public sealed record CreateTaskCommand(Guid ProjectId, string Title) : ICommand<CreateTaskResponse>
#if (mediator == "mediatr")
    , MediatR.IRequest<CreateTaskResponse>
#endif
;
