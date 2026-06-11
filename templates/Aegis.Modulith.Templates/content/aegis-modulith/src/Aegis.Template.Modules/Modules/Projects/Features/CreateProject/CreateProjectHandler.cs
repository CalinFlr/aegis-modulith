using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.Projects.Domain;
using Aegis.Template.Modules.Modules.Projects.Infrastructure;

namespace Aegis.Template.Modules.Modules.Projects.Features.CreateProject;

public sealed class CreateProjectHandler(ProjectsDbContext dbContext) :
    ICommandHandler<CreateProjectCommand, CreateProjectResponse>
#if (mediator == "mediatr")
    , MediatR.IRequestHandler<CreateProjectCommand, CreateProjectResponse>
#endif
{
    public async Task<CreateProjectResponse> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var project = Project.Create(command.Name);
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateProjectResponse(project.Id, project.Name, project.CreatedAtUtc);
    }

#if (mediator == "mediatr")
    async Task<CreateProjectResponse> MediatR.IRequestHandler<CreateProjectCommand, CreateProjectResponse>.Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
