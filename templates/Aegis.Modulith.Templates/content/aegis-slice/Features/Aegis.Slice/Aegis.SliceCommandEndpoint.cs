using AegisBuildingBlocksNamespace.Cqrs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

public static class Aegis.SliceCommandEndpoint
{
    public static RouteGroupBuilder MapAegis.Slice(this RouteGroupBuilder group)
    {
        group.MapPost("/Aegis.Slice", async Task<IResult> (
            Aegis.SliceCommand command,
            ICommandDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = new Aegis.SliceCommandValidator().Validate(command);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors.ToDictionary(
                    error => error,
                    error => new[] { error }));
            }

            var response = await dispatcher.Send(command, cancellationToken);
            return Results.Ok(response);
        }).WithName("Aegis.Slice");

        return group;
    }
}
