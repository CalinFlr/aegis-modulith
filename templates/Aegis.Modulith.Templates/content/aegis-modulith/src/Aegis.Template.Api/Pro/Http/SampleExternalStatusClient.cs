namespace Aegis.Template.Api.Pro.Http;

public sealed class SampleExternalStatusClient(HttpClient httpClient)
{
    public Task<HttpResponseMessage> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        // Placeholder pattern for module-owned external clients. Replace the base address in configuration
        // before calling this in real code; the template does not depend on an external service.
        return httpClient.GetAsync("status", cancellationToken);
    }
}
