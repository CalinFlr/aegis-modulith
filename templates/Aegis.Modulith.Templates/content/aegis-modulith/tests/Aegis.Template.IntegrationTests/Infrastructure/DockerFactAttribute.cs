namespace Aegis.Template.IntegrationTests.Infrastructure;

/// <summary>
/// Runs Docker-backed integration tests only when explicitly enabled.
/// Set AEGIS_RUN_TESTCONTAINERS=true and make sure local Docker is available.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class DockerFactAttribute : FactAttribute
{
    public DockerFactAttribute()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("AEGIS_RUN_TESTCONTAINERS"), "true", StringComparison.OrdinalIgnoreCase))
        {
            Skip = "Requires local Docker. Set AEGIS_RUN_TESTCONTAINERS=true to run this Testcontainers test.";
        }
    }
}
