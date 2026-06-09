using AegisBuildingBlocksNamespace.Cqrs;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

public sealed class Aegis.SliceCommandHandler :
    ICommandHandler<Aegis.SliceCommand, Aegis.SliceResponse>
#if (mediator == "mediatr")
    , MediatR.IRequestHandler<Aegis.SliceCommand, Aegis.SliceResponse>
#endif
{
    public Task<Aegis.SliceResponse> Handle(Aegis.SliceCommand command, CancellationToken cancellationToken)
    {
        // Replace this placeholder with module domain behavior and handler tests.
        var response = new Aegis.SliceResponse(Guid.NewGuid(), command.Name.Trim(), DateTimeOffset.UtcNow);
        return Task.FromResult(response);
    }

#if (mediator == "mediatr")
    Task<Aegis.SliceResponse> MediatR.IRequestHandler<Aegis.SliceCommand, Aegis.SliceResponse>.Handle(
        Aegis.SliceCommand request,
        CancellationToken cancellationToken) => Handle(request, cancellationToken);
#endif
}
