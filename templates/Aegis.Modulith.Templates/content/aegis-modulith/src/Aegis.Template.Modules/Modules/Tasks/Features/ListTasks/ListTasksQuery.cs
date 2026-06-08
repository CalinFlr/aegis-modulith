using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.Tasks.Features.ListTasks;

public sealed record ListTasksQuery : IQuery<IReadOnlyList<ListTasksResponse>>
#if (mediator == "mediatr")
    , MediatR.IRequest<IReadOnlyList<ListTasksResponse>>
#endif
;
