using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.WorkItems.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.WorkItems.Features.GetWorkItemById;

public sealed class GetWorkItemByIdHandler(WorkItemsDbContext dbContext) :
    IQueryHandler<GetWorkItemByIdQuery, GetWorkItemByIdResponse?>
#if (mediator == "mediatr")
    , MediatR.IRequestHandler<GetWorkItemByIdQuery, GetWorkItemByIdResponse?>
#endif
{
    public async Task<GetWorkItemByIdResponse?> Handle(GetWorkItemByIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.WorkItems
            .AsNoTracking()
            .Where(workItem => workItem.Id == query.Id)
            .Select(workItem => new GetWorkItemByIdResponse(workItem.Id, workItem.Title, workItem.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }

#if (mediator == "mediatr")
    async Task<GetWorkItemByIdResponse?> MediatR.IRequestHandler<GetWorkItemByIdQuery, GetWorkItemByIdResponse?>.Handle(
        GetWorkItemByIdQuery request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
