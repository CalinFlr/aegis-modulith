using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.Projects.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Projects.Features.GetProjectById;

public sealed class GetProjectByIdHandler(ProjectsDbContext dbContext) :
    IQueryHandler<GetProjectByIdQuery, GetProjectByIdResponse?>
#if AEGIS_MEDIATR
    , MediatR.IRequestHandler<GetProjectByIdQuery, GetProjectByIdResponse?>
#endif
{
    public async Task<GetProjectByIdResponse?> Handle(GetProjectByIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Id == query.Id)
            .Select(project => new GetProjectByIdResponse(project.Id, project.Name, project.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }

#if AEGIS_MEDIATR
    async Task<GetProjectByIdResponse?> MediatR.IRequestHandler<GetProjectByIdQuery, GetProjectByIdResponse?>.Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
