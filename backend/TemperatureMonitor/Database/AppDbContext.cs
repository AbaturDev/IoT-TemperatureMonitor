using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.Database.Entities;

namespace TemperatureMonitor.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Measurement> Measurements { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Measurement>()
            .HasIndex(m => m.Id);
    }
}