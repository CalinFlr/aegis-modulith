namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class InboxProcessorWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<InboxProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Inbox processor worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<InboxProcessor>();

            await processor.ProcessPendingAsync(cancellationToken: stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
