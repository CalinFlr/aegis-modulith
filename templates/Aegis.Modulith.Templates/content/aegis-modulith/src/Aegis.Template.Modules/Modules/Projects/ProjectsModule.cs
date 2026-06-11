using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.BuildingBlocks.Modules;
using Aegis.Template.Modules.Modules.Projects.Features.CreateProject;
using Aegis.Template.Modules.Modules.Projects.Features.GetProjectById;
using Aegis.Template.Modules.Modules.Projects.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Template.Modules.Modules.Projects;

public sealed class ProjectsModule : IAegisModule
{
    public string Name => "Projects";

    public string Schema => "projects";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres must be configured for the Projects module.");

        services.AddDbContext<ProjectsDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/projects").WithTags(Name);

        group.MapPost("/", async (CreateProjectCommand command, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(command, cancellationToken);
            return Results.Created($"/projects/{response.Id}", response);
        }).WithName("CreateProject");

        group.MapGet("/{id:guid}", async (Guid id, IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(new GetProjectByIdQuery(id), cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        }).WithName("GetProjectById");
    }
}
