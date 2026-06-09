using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages", AegisInboxDbContext.Schema);
        builder.HasKey(message => message.Id);
        builder.HasIndex(message => message.MessageId).IsUnique();
        builder.HasIndex(message => message.IdempotencyKey).IsUnique();
        builder.HasIndex(message => new { message.Status, message.ReceivedAtUtc });

        builder.Property(message => message.IdempotencyKey).HasMaxLength(128).IsRequired();
        builder.Property(message => message.MessageType).HasMaxLength(512).IsRequired();
        builder.Property(message => message.Payload).IsRequired();
        builder.Property(message => message.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(message => message.FailureReason).HasMaxLength(2048);
    }
}
