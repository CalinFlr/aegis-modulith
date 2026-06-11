using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.Tasks.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Tasks.Features.ListTasks;

public sealed class ListTasksHandler(TasksDbContext dbContext) :
    IQueryHandler<ListTasksQuery, IReadOnlyList<ListTasksResponse>>
#if AEGIS_MEDIATR
    , MediatR.IRequestHandler<ListTasksQuery, IReadOnlyList<ListTasksResponse>>
#endif
{
    public async Task<IReadOnlyList<ListTasksResponse>> Handle(ListTasksQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.CreatedAtUtc)
            .Select(task => new ListTasksResponse(task.Id, task.ProjectId, task.Title, task.CreatedAtUtc))
            .ToArrayAsync(cancellationToken);
    }

#if AEGIS_MEDIATR
    async Task<IReadOnlyList<ListTasksResponse>> MediatR.IRequestHandler<ListTasksQuery, IReadOnlyList<ListTasksResponse>>.Handle(
        ListTasksQuery request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
