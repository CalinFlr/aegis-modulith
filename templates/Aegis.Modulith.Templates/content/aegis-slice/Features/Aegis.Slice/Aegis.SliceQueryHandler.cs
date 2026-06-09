using AegisBuildingBlocksNamespace.Cqrs;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

#if (paged)
public sealed class Aegis.SliceQueryHandler :
    IQueryHandler<Aegis.SliceQuery, Aegis.SliceResponse>
#if (mediator == "mediatr")
    , MediatR.IRequestHandler<Aegis.SliceQuery, Aegis.SliceResponse>
#endif
{
    public Task<Aegis.SliceResponse> Handle(Aegis.SliceQuery query, CancellationToken cancellationToken)
    {
        // Replace this placeholder with a no-tracking projection and query handler tests.
        var pageNumber = Math.Max(1, query.PageNumber);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var response = new Aegis.SliceResponse(Array.Empty<Aegis.SliceItemResponse>(), pageNumber, pageSize, 0);
        return Task.FromResult(response);
    }

#if (mediator == "mediatr")
    Task<Aegis.SliceResponse> MediatR.IRequestHandler<Aegis.SliceQuery, Aegis.SliceResponse>.Handle(
        Aegis.SliceQuery request,
        CancellationToken cancellationToken) => Handle(request, cancellationToken);
#endif
}
#else
public sealed class Aegis.SliceQueryHandler :
    IQueryHandler<Aegis.SliceQuery, Aegis.SliceResponse?>
#if (mediator == "mediatr")
    , MediatR.IRequestHandler<Aegis.SliceQuery, Aegis.SliceResponse?>
#endif
{
    public Task<Aegis.SliceResponse?> Handle(Aegis.SliceQuery query, CancellationToken cancellationToken)
    {
        // Replace this placeholder with a no-tracking projection and query handler tests.
        Aegis.SliceResponse? response = null;
        return Task.FromResult(response);
    }

#if (mediator == "mediatr")
    Task<Aegis.SliceResponse?> MediatR.IRequestHandler<Aegis.SliceQuery, Aegis.SliceResponse?>.Handle(
        Aegis.SliceQuery request,
        CancellationToken cancellationToken) => Handle(request, cancellationToken);
#endif
}
#endif
