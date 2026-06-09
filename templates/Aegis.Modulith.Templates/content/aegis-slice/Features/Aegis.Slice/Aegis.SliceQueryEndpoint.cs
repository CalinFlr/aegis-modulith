using AegisBuildingBlocksNamespace.Cqrs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

public static class Aegis.SliceQueryEndpoint
{
    public static RouteGroupBuilder MapAegis.Slice(this RouteGroupBuilder group)
    {
#if (paged)
        group.MapGet("/Aegis.Slice", async Task<IResult> (
            [AsParameters] Aegis.SliceQuery query,
            IQueryDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(query, cancellationToken);
            return Results.Ok(response);
        }).WithName("Aegis.Slice");
#else
        group.MapGet("/Aegis.Slice/{id:guid}", async Task<IResult> (
            Guid id,
            IQueryDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var response = await dispatcher.Send(new Aegis.SliceQuery(id), cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        }).WithName("Aegis.Slice");
#endif

        return group;
    }
}
