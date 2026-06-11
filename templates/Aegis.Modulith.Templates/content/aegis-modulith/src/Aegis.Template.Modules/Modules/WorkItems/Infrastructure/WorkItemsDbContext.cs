using Aegis.Template.Modules.Modules.WorkItems.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.WorkItems.Infrastructure;

public sealed class WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("work_items");
        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.ToTable("work_items");
            entity.HasKey(workItem => workItem.Id);
            entity.Property(workItem => workItem.Title).HasMaxLength(200).IsRequired();
            entity.Ignore(workItem => workItem.DomainEvents);
        });
    }
}
