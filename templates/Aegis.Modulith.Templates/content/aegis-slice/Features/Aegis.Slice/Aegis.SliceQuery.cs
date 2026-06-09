using AegisBuildingBlocksNamespace.Cqrs;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

#if (paged)
public sealed record Aegis.SliceQuery(int PageNumber = 1, int PageSize = 50) : IQuery<Aegis.SliceResponse>
#if (mediator == "mediatr")
    , MediatR.IRequest<Aegis.SliceResponse>
#endif
;
#else
public sealed record Aegis.SliceQuery(Guid Id) : IQuery<Aegis.SliceResponse?>
#if (mediator == "mediatr")
    , MediatR.IRequest<Aegis.SliceResponse?>
#endif
;
#endif
