using Microsoft.IdentityModel.Tokens;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
using TemperatureMonitor.Database.Enums;

namespace TemperatureMonitor.BackgroundServices;

public class SnapshotBackgroundService : BackgroundService
{
    private readonly ILogger<SnapshotBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private const int IntervalMs = 300_000; // 5 min
    
    public SnapshotBackgroundService(ILogger<SnapshotBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    private async Task CreateSnapshotAsync(AppDbContext context, CancellationToken ct)
    {
        var currentDate = DateTime.Now;
        
        var measurements = context.Measurements.Where(x => x.Status == MeasurementStatus.Success && x.Timestamp >= currentDate - TimeSpan.FromMilliseconds(IntervalMs, 0)).ToList();

        if (measurements.IsNullOrEmpty())
        {
            _logger.LogWarning("Brak pomiarów do stworzenia snapshotu");
            return;
        }

        var measurementSnapshot = new MeasurementSnapshot
        {
            Timestamp = measurements.Max(x => x.Timestamp),
            Temperature = measurements.Average(x => x.Temperature)!.Value,
            Humidity = measurements.Average(x => x.Humidity)!.Value,
        };

        await context.MeasurementSnapshots.AddAsync(measurementSnapshot, ct);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        while (!ct.IsCancellationRequested)
        {
            await CreateSnapshotAsync(context, ct);
            await Task.Delay(IntervalMs, ct);
        }
    }
}