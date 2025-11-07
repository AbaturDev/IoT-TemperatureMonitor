using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.Database.Entities;

namespace TemperatureMonitor.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Measurement> Measurements { get; init; }
    public DbSet<MeasurementSnapshot> MeasurementSnapshots { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Measurement>()
            .HasIndex(m => m.Id);
        
        modelBuilder.Entity<MeasurementSnapshot>()
            .HasIndex(ms => ms.Id);
    }
}