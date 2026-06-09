using AegisBuildingBlocksNamespace.Cqrs;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

public sealed record Aegis.SliceCommand(string Name) : ICommand<Aegis.SliceResponse>
#if (mediator == "mediatr")
    , MediatR.IRequest<Aegis.SliceResponse>
#endif
;
