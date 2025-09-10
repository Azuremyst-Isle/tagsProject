using Microsoft.EntityFrameworkCore;
using RfidApi.Models;

namespace RfidApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Item> item => Set<Item>();
}
