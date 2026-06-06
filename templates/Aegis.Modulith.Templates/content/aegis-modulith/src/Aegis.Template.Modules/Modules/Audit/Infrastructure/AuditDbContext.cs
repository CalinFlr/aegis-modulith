using Aegis.Template.Modules.Modules.Audit.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Audit.Infrastructure;

public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("audit");
        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.ToTable("audit_entries");
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.Stream).HasMaxLength(200).IsRequired();
            entity.Property(entry => entry.Action).HasMaxLength(200).IsRequired();
        });
    }
}
