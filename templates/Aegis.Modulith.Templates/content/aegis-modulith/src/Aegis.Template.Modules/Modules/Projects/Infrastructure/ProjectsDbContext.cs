using Aegis.Template.Modules.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Projects.Infrastructure;

public sealed class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("projects");
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Name).HasMaxLength(200).IsRequired();
            entity.Ignore(project => project.DomainEvents);
        });
    }
}
