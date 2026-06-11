using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules.Modules.WorkItems.Domain;
using Aegis.Template.Modules.Modules.WorkItems.Infrastructure;

namespace Aegis.Template.Modules.Modules.WorkItems.Features.CreateWorkItem;

public sealed class CreateWorkItemHandler(WorkItemsDbContext dbContext) :
    ICommandHandler<CreateWorkItemCommand, CreateWorkItemResponse>
#if AEGIS_MEDIATR
    , MediatR.IRequestHandler<CreateWorkItemCommand, CreateWorkItemResponse>
#endif
{
    public async Task<CreateWorkItemResponse> Handle(CreateWorkItemCommand command, CancellationToken cancellationToken)
    {
        var workItem = WorkItem.Create(command.Title);
        dbContext.WorkItems.Add(workItem);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateWorkItemResponse(workItem.Id, workItem.Title, workItem.CreatedAtUtc);
    }

#if AEGIS_MEDIATR
    async Task<CreateWorkItemResponse> MediatR.IRequestHandler<CreateWorkItemCommand, CreateWorkItemResponse>.Handle(
        CreateWorkItemCommand request,
        CancellationToken cancellationToken) => await Handle(request, cancellationToken);
#endif
}
