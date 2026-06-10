using System.Diagnostics;
using System.Net;

namespace Aegis.Template.PerformanceSmokeTests.Infrastructure;

internal static class PerformanceSmokeAssertions
{
    public static async Task<TimeSpan> MeasureOnceAsync(string scenario, Func<Task> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        await operation();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    public static async Task<TimeSpan[]> MeasureWarmedSamplesAsync(Func<Task<HttpResponseMessage>> request)
    {
        for (var iteration = 0; iteration < PerformanceSmokeThresholds.WarmupIterations; iteration++)
        {
            using var warmup = await request();
            Assert.True(
                IsExpectedSmokeStatus(warmup.StatusCode),
                $"Warm-up request returned {(int)warmup.StatusCode} {warmup.StatusCode}.");
        }

        var samples = new List<TimeSpan>();
        for (var iteration = 0; iteration < PerformanceSmokeThresholds.MeasuredIterations; iteration++)
        {
            var elapsed = await MeasureOnceAsync("warmed request", async () =>
            {
                using var response = await request();
                Assert.True(
                    IsExpectedSmokeStatus(response.StatusCode),
                    $"Measured request returned {(int)response.StatusCode} {response.StatusCode}.");
            });

            samples.Add(elapsed);
        }

        return samples.ToArray();
    }

    public static void AssertBestSampleWithin(string scenario, IReadOnlyCollection<TimeSpan> samples, TimeSpan threshold)
    {
        Assert.NotEmpty(samples);
        var best = samples.Min();
        Assert.True(
            best <= threshold,
            $"{scenario} best warmed sample was {Format(best)}. Loose diagnostic threshold is {Format(threshold)}. " +
            $"All samples: {string.Join(", ", samples.Select(Format))}. " +
            "This is a performance smoke test for obvious regressions, not a benchmark or production performance certification.");
    }

    public static void AssertWithin(string scenario, TimeSpan elapsed, TimeSpan threshold)
    {
        Assert.True(
            elapsed <= threshold,
            $"{scenario} took {Format(elapsed)}. Loose diagnostic threshold is {Format(threshold)}. " +
            "This is a performance smoke test for obvious regressions, not a benchmark or production performance certification.");
    }

    private static bool IsExpectedSmokeStatus(HttpStatusCode statusCode)
    {
        return statusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices
            or HttpStatusCode.NotFound;
    }

    private static string Format(TimeSpan value)
    {
        return $"{value.TotalMilliseconds:N0} ms";
    }
}
