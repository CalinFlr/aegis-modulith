using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.Tasks.Domain;
using Aegis.Template.Modules.Modules.Tasks.Infrastructure;

namespace Aegis.Template.Modules.Modules.Tasks.Features.CreateTask;

public sealed class CreateTaskHandler(TasksDbContext dbContext) :
    ICommandHandler<CreateTaskCommand, CreateTaskResponse>
#if AEGIS_MEDIATR
    , MediatR.IRequestHandler<CreateTaskCommand, CreateTaskResponse>
#endif
{
    public async Task<CreateTaskResponse> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var task = TaskItem.Create(command.ProjectId, command.Title);
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateTaskResponse(task.Id, task.ProjectId, task.Title, task.CreatedAtUtc);
    }

#if AEGIS_MEDIATR
    async Task<CreateTaskResponse> MediatR.IRequestHandler<CreateTaskCommand, CreateTaskResponse>.Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
