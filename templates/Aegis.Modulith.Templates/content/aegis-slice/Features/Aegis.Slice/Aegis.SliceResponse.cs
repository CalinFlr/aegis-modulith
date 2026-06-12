namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

#if (kind == "query")
#if (paged)
public sealed record Aegis.SliceItemResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);

public sealed record Aegis.SliceResponse(
    IReadOnlyList<Aegis.SliceItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);
#else
public sealed record Aegis.SliceResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);
#endif
#else
public sealed record Aegis.SliceResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);
#endif
