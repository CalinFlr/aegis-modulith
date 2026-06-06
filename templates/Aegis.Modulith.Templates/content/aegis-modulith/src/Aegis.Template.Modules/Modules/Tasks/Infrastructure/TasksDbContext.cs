using Aegis.Template.Modules.Modules.Tasks.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Tasks.Infrastructure;

public sealed class TasksDbContext(DbContextOptions<TasksDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tasks");
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title).HasMaxLength(200).IsRequired();
            entity.Ignore(task => task.DomainEvents);
            entity.HasIndex(task => task.ProjectId);
        });
    }
}
