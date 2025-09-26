using Microsoft.EntityFrameworkCore;
using RfidApi.Models;

namespace RfidApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Item> item => Set<Item>();

    public DbSet<Users> Users => Set<Users>();

    public DbSet<ItemEvent> ItemEvents => Set<ItemEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>().Property(i => i.status).HasDefaultValue("available");

        modelBuilder
            .Entity<Item>()
            .Property(i => i.last_updated)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder
            .Entity<Item>()
            .HasOne(i => i.OwnerUser)
            .WithMany()
            .HasForeignKey(i => i.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Item>().HasIndex(i => i.OwnerUserId);

        modelBuilder.Entity<Item>().ToTable("items");

        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Email).HasColumnName("email");
            entity.Property(u => u.Name).HasColumnName("name");
            entity.Property(u => u.Role).HasColumnName("role");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
        });
        modelBuilder
            .Entity<Users>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ItemEvent>().HasIndex(e => e.ItemId);
        modelBuilder.Entity<ItemEvent>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RfidTag).HasColumnName("rfid_tag");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.EventPayload).HasColumnName("event_payload");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ActorEmail).HasColumnName("actor_email");
        });
        modelBuilder
            .Entity<ItemEvent>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
