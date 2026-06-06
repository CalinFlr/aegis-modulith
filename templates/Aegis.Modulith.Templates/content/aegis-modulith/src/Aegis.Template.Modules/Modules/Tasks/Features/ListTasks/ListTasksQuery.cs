using Aegis.Template.BuildingBlocks.Cqrs;

namespace Aegis.Template.Modules.Modules.Tasks.Features.ListTasks;

public sealed record ListTasksQuery : IQuery<IReadOnlyList<ListTasksResponse>>
#if AEGIS_MEDIATR
    , MediatR.IRequest<IReadOnlyList<ListTasksResponse>>
#endif
;
