namespace Aegis.Template.PerformanceSmokeTests.Infrastructure;

internal static class PerformanceSmokeThresholds
{
    public const int WarmupIterations = 1;
    public const int MeasuredIterations = 3;

    // These are intentionally loose smoke thresholds, not benchmark guarantees.
    public static readonly TimeSpan HostStartup = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan SimpleRequest = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan AuthenticatedRequest = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan CqrsDispatchRequest = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan OpenApiGeneration = TimeSpan.FromSeconds(10);
}
