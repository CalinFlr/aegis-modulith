namespace AegisSliceModule.Features.Aegis.Slice;

public sealed class Aegis.SliceHandler
{
    public Task<Aegis.SliceResponse> Handle(Aegis.SliceRequest request, CancellationToken cancellationToken)
    {
        var response = new Aegis.SliceResponse(Guid.NewGuid(), request.Value, "AegisSliceKind");
        return Task.FromResult(response);
    }
}
