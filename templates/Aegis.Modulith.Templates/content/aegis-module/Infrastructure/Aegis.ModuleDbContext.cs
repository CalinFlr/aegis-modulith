using AegisItemRootNamespace.Modules.Aegis.Module.Domain;
using Microsoft.EntityFrameworkCore;

namespace AegisItemRootNamespace.Modules.Aegis.Module.Infrastructure;

public sealed class Aegis.ModuleDbContext(DbContextOptions<Aegis.ModuleDbContext> options) : DbContext(options)
{
    public const string Schema = "aegis_module_schema";

    public DbSet<Aegis.ModuleEntity> Aegis.ModuleEntities => Set<Aegis.ModuleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Aegis.ModuleEntity>(entity =>
        {
            entity.ToTable("aegis_module_entities");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(200).IsRequired();
        });
    }
}
