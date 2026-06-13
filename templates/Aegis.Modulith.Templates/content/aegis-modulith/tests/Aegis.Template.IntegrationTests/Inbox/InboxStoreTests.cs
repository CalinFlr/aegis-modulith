#if (sample == "taskhub")
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.Tasks.Contracts.TaskCreatedIntegrationEvent;
#else
using SampleIntegrationEvent = Aegis.Template.Modules.Modules.WorkItems.Contracts.WorkItemCreatedIntegrationEvent;
#endif
using System.Text.Json;
using Aegis.Template.Api.Pro.Infrastructure.Inbox;
using Aegis.Template.BuildingBlocks.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aegis.Template.IntegrationTests.Inbox;

public sealed class InboxStoreTests
{
    [Fact]
    public async Task First_message_is_accepted()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();

        var result = await store.AcceptAsync(message.Id, messageType, JsonSerializer.Serialize(message));

        Assert.Equal(InboxAcceptResult.Accepted, result);
        Assert.Single(await dbContext.InboxMessages.ToArrayAsync());
    }

    [Fact]
    public async Task Duplicate_message_id_is_detected()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();
        var payload = JsonSerializer.Serialize(message);
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();

        var first = await store.AcceptAsync(message.Id, messageType, payload);
        var duplicate = await store.AcceptAsync(message.Id, messageType, payload);

        Assert.Equal(InboxAcceptResult.Accepted, first);
        Assert.Equal(InboxAcceptResult.Duplicate, duplicate);
        Assert.True(await store.IsDuplicateAsync(message.Id));
        Assert.Single(await dbContext.InboxMessages.ToArrayAsync());
    }

    [Fact]
    public async Task Processed_message_is_not_accepted_again()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();
        var payload = JsonSerializer.Serialize(message);
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();

        await store.AcceptAsync(message.Id, messageType, payload);
        await store.TryBeginProcessingAsync(message.Id);
        await store.MarkProcessedAsync(message.Id);

        var duplicate = await store.AcceptAsync(message.Id, messageType, payload);

        Assert.Equal(InboxAcceptResult.AlreadyProcessed, duplicate);
        var stored = await dbContext.InboxMessages.SingleAsync();
        Assert.Equal(InboxMessageStatus.Processed, stored.Status);
        Assert.NotNull(stored.ProcessedAtUtc);
    }

    [Fact]
    public async Task Failed_message_records_failure_and_retry_state()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();

        await store.AcceptAsync(message.Id, messageType, JsonSerializer.Serialize(message));
        await store.TryBeginProcessingAsync(message.Id);
        await store.MarkFailedAsync(message.Id, "handler failed");

        var stored = await dbContext.InboxMessages.SingleAsync();
        Assert.Equal(InboxMessageStatus.Failed, stored.Status);
        Assert.Equal("handler failed", stored.FailureReason);
        Assert.Equal(1, stored.AttemptCount);
    }

    [Fact]
    public async Task Pending_scan_includes_processing_messages_after_lease_expires()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();

        await store.AcceptAsync(message.Id, typeof(SampleIntegrationEvent).FullName!, JsonSerializer.Serialize(message));
        var stored = await dbContext.InboxMessages.SingleAsync();
        stored.MarkProcessing(Guid.NewGuid(), DateTimeOffset.UtcNow.AddMinutes(-1));
        await dbContext.SaveChangesAsync();

        var pending = await store.GetPendingAsync(maxMessages: 10);

        Assert.Contains(pending, candidate => candidate.MessageId == message.Id);
    }

    [Fact]
    public async Task Active_processing_lease_is_not_claimed_again()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var message = CreateSampleEvent();

        await store.AcceptAsync(message.Id, typeof(SampleIntegrationEvent).FullName!, JsonSerializer.Serialize(message));
        var firstClaim = await store.TryBeginProcessingAsync(message.Id);
        var secondClaim = await store.TryBeginProcessingAsync(message.Id);

        Assert.NotNull(firstClaim);
        Assert.Null(secondClaim);
    }

    [Fact]
    public async Task Processor_invokes_handler_once_for_duplicate_inputs()
    {
        await using var dbContext = CreateDbContext();
        var store = CreateStore(dbContext);
        var messageType = IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>();
        var handler = new CountingInboxMessageHandler(messageType);
        var processor = new InboxProcessor(store, [handler], CreateLogger<InboxProcessor>());
        var message = CreateSampleEvent();
        var payload = JsonSerializer.Serialize(message);

        await store.AcceptAsync(message.Id, messageType, payload);
        await store.AcceptAsync(message.Id, messageType, payload);

        var processed = await processor.ProcessPendingAsync();
        var secondPass = await processor.ProcessPendingAsync();

        Assert.Equal(1, processed);
        Assert.Equal(0, secondPass);
        Assert.Equal(1, handler.HandleCount);
    }

    private static AegisInboxDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AegisInboxDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AegisInboxDbContext(options);
    }

    private static EfCoreInboxStore CreateStore(AegisInboxDbContext dbContext)
    {
        return new EfCoreInboxStore(dbContext, CreateLogger<EfCoreInboxStore>());
    }

    private static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.Create(_ => { }).CreateLogger<T>();
    }

    private static SampleIntegrationEvent CreateSampleEvent()
    {
#if (sample == "taskhub")
        return new SampleIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
#else
        return new SampleIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
#endif
    }

    private sealed class CountingInboxMessageHandler(string messageType) : IInboxMessageHandler
    {
        public string MessageType => messageType;

        public int HandleCount { get; private set; }

        public Task HandleAsync(InboxMessage message, CancellationToken cancellationToken)
        {
            HandleCount++;
            return Task.CompletedTask;
        }
    }
}
