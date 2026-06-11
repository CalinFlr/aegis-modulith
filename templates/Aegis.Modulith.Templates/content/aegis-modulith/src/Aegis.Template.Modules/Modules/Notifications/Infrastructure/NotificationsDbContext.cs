using Aegis.Template.Modules.Modules.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aegis.Template.Modules.Modules.Notifications.Infrastructure;

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notifications");
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(notification => notification.Id);
            entity.Property(notification => notification.Recipient).HasMaxLength(200).IsRequired();
            entity.Property(notification => notification.Message).HasMaxLength(1000).IsRequired();
        });
    }
}
