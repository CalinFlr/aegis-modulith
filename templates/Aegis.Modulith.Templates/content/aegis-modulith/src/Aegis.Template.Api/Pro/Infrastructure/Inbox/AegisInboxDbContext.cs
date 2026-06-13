using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Api.Pro.Infrastructure.Inbox;

public sealed class AegisInboxDbContext(DbContextOptions<AegisInboxDbContext> options) : DbContext(options)
{
    public const string Schema = "integration";

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
