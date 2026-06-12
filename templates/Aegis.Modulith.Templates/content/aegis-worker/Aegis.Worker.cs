using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aegis.Worker;

public sealed class Aegis.Worker(ILogger<Aegis.Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{WorkerName} for module {ModuleName} started.", nameof(Aegis.Worker), "AegisWorkerModule");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private Task ProcessAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("{WorkerName} for module {ModuleName} completed one background pass.", nameof(Aegis.Worker), "AegisWorkerModule");
        return Task.CompletedTask;
    }
}
