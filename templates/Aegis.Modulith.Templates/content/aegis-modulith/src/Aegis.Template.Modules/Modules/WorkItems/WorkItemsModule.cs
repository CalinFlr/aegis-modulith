#if (profile != "core")
using Aegis.Template.BuildingBlocks.Authorization;
using Microsoft.AspNetCore.Builder;
#endif
using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.BuildingBlocks.Modules;
using Aegis.Template.Modules.Modules.WorkItems.Features.CreateWorkItem;
using Aegis.Template.Modules.Modules.WorkItems.Features.GetWorkItemById;
using Aegis.Template.Modules.Modules.WorkItems.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules.Modules.WorkItems;

public sealed class WorkItemsModule : IAegisModule
{
    public string Name => "WorkItems";

    public string Schema => "work_items";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:Postgres must be configured for the WorkItems module.");
        }

        services.AddDbContext<WorkItemsDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/work-items").WithTags(Name);

        group.MapPost("/", async (CreateWorkItemCommand command, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(command, cancellationToken);
            return Results.Created($"/work-items/{response.Id}", response);
        }).WithName("CreateWorkItem")
            .Accepts<CreateWorkItemCommand>("application/json")
            .Produces<CreateWorkItemResponse>(StatusCodes.Status201Created)
#if (profile != "core")
            .RequireAuthorization(AegisAuthorizationPolicies.WorkItemsWrite)
#endif
            ;

        group.MapGet("/{id:guid}", async (Guid id, IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(new GetWorkItemByIdQuery(id), cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        }).WithName("GetWorkItemById")
            .Produces<GetWorkItemByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
#if (profile != "core")
            .RequireAuthorization(AegisAuthorizationPolicies.WorkItemsRead)
#endif
            ;
    }
}
