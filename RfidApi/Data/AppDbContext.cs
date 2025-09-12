using Microsoft.EntityFrameworkCore;
using RfidApi.Models;

namespace RfidApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Item> item => Set<Item>();

    public DbSet<Users> Users => Set<Users>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>().Property(i => i.status).HasDefaultValue("available");

        modelBuilder
            .Entity<Item>()
            .Property(i => i.last_updated)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Item>().ToTable("TagItems");
    }
}
