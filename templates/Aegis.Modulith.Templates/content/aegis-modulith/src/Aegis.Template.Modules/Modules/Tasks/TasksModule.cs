using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.BuildingBlocks.Modules;
using Aegis.Template.Modules.Modules.Tasks.Features.CreateTask;
using Aegis.Template.Modules.Modules.Tasks.Features.ListTasks;
using Aegis.Template.Modules.Modules.Tasks.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules.Modules.Tasks;

public sealed class TasksModule : IAegisModule
{
    public string Name => "Tasks";

    public string Schema => "tasks";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres must be configured for the Tasks module.");

        services.AddDbContext<TasksDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/tasks").WithTags(Name);

        group.MapPost("/", async (CreateTaskCommand command, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(command, cancellationToken);
            return Results.Created($"/tasks/{response.Id}", response);
        }).WithName("CreateTask");

        group.MapGet("/", async (IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(new ListTasksQuery(), cancellationToken);
            return Results.Ok(response);
        }).WithName("ListTasks");
    }
}
