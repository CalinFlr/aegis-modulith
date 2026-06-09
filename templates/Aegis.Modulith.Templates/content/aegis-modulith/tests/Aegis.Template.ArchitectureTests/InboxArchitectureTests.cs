namespace Aegis.Template.ArchitectureTests;

public sealed class InboxArchitectureTests
{
    [Fact]
    public void Inbox_profile_wiring_matches_selected_profile()
    {
        var inboxRoot = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "Pro", "Infrastructure", "Inbox");

        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            Assert.False(Directory.Exists(inboxRoot), "Core profile must not include active inbox infrastructure.");
            return;
        }

        Assert.True(Directory.Exists(inboxRoot), "Pro and advanced profiles must include inbox infrastructure.");
        Assert.True(File.Exists(Path.Combine(inboxRoot, "InboxMessage.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "AegisInboxDbContext.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "InboxMessageConfiguration.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "IInboxStore.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "EfCoreInboxStore.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "InboxProcessor.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "IInboxMessageHandler.cs")));
        Assert.True(File.Exists(Path.Combine(inboxRoot, "SampleIntegrationEventInboxHandler.cs")));

        var proServices = File.ReadAllText(Path.Combine(ArchitectureTestContext.ApiProjectRoot, "Pro", "ProProfileServices.cs"));
        Assert.Contains("AddAegisInbox(configuration)", proServices, StringComparison.Ordinal);
    }

    [Fact]
    public void Inbox_persistence_uses_unique_message_identity()
    {
        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            return;
        }

        var inboxRoot = Path.Combine(ArchitectureTestContext.ApiProjectRoot, "Pro", "Infrastructure", "Inbox");
        var entity = File.ReadAllText(Path.Combine(inboxRoot, "InboxMessage.cs"));
        var configuration = File.ReadAllText(Path.Combine(inboxRoot, "InboxMessageConfiguration.cs"));
        var dbContext = File.ReadAllText(Path.Combine(inboxRoot, "AegisInboxDbContext.cs"));

        Assert.Contains("MessageId", entity, StringComparison.Ordinal);
        Assert.Contains("IdempotencyKey", entity, StringComparison.Ordinal);
        Assert.Contains("MessageType", entity, StringComparison.Ordinal);
        Assert.Contains("Payload", entity, StringComparison.Ordinal);
        Assert.Contains("ReceivedAtUtc", entity, StringComparison.Ordinal);
        Assert.Contains("ProcessedAtUtc", entity, StringComparison.Ordinal);
        Assert.Contains("FailureReason", entity, StringComparison.Ordinal);
        Assert.Contains("AttemptCount", entity, StringComparison.Ordinal);
        Assert.Contains("HasIndex(message => message.MessageId).IsUnique()", configuration, StringComparison.Ordinal);
        Assert.Contains("HasIndex(message => message.IdempotencyKey).IsUnique()", configuration, StringComparison.Ordinal);
        Assert.Contains("ToTable(\"inbox_messages\", AegisInboxDbContext.Schema)", configuration, StringComparison.Ordinal);
        Assert.Contains("Schema = \"integration\"", dbContext, StringComparison.Ordinal);
        Assert.Contains("DbSet<InboxMessage>", dbContext, StringComparison.Ordinal);
    }

    [Fact]
    public void Domain_code_does_not_reference_inbox_infrastructure()
    {
        var failures = new List<string>();

        foreach (var moduleFolder in ArchitectureTestContext.ModuleFolders)
        {
            var domainFolder = Path.Combine(moduleFolder, "Domain");
            foreach (var sourceFile in ArchitectureTestContext.SourceFilesUnder(domainFolder))
            {
                var content = File.ReadAllText(sourceFile);
                if (content.Contains(".Infrastructure.Inbox", StringComparison.Ordinal) ||
                    content.Contains("InboxMessage", StringComparison.Ordinal))
                {
                    failures.Add($"{ArchitectureTestContext.Relative(sourceFile)} references inbox infrastructure.");
                }
            }
        }

        Assert.Empty(failures);
    }

    [Fact]
    public void Inbox_processing_uses_integration_event_contracts()
    {
        if (ArchitectureTestContext.GetOption("AegisProfile") == "core")
        {
            return;
        }

        var handler = File.ReadAllText(Path.Combine(
            ArchitectureTestContext.ApiProjectRoot,
            "Pro",
            "Infrastructure",
            "Inbox",
            "SampleIntegrationEventInboxHandler.cs"));

        Assert.Contains(".Contracts", handler, StringComparison.Ordinal);
        Assert.Contains("IntegrationEvent", handler, StringComparison.Ordinal);
        Assert.DoesNotContain(".Domain", handler, StringComparison.Ordinal);
    }

    [Fact]
    public void Generated_inbox_does_not_reference_a_broker_by_default()
    {
        var forbiddenBrokerMarkers = new[]
        {
            "MassTransit",
            "RabbitMQ",
            "Confluent.Kafka",
            "Azure.Messaging.ServiceBus",
            "NServiceBus"
        };
        var sourceFiles = Directory.GetFiles(ArchitectureTestContext.SourceRoot, "*.cs", SearchOption.AllDirectories);

        foreach (var sourceFile in sourceFiles)
        {
            var content = File.ReadAllText(sourceFile);
            foreach (var marker in forbiddenBrokerMarkers)
            {
                Assert.DoesNotContain(marker, content, StringComparison.Ordinal);
            }
        }
    }
}
