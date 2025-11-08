using System.Threading.Channels;
using Microsoft.IdentityModel.Tokens;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
using TemperatureMonitor.Database.Enums;

namespace TemperatureMonitor.BackgroundServices;

public class SnapshotBackgroundService : BackgroundService
{
    private readonly ILogger<SnapshotBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<MeasurementSnapshot> _channel;

    private const int IntervalMs = 300_000; // 5 min
    
    public SnapshotBackgroundService(ILogger<SnapshotBackgroundService> logger, IServiceProvider serviceProvider, Channel<MeasurementSnapshot> channel)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _channel = channel;
    }

    private async Task<MeasurementSnapshot?> CreateSnapshotAsync(AppDbContext context, CancellationToken ct)
    {
        var currentDate = DateTimeOffset.UtcNow;
        
        var measurements = context.Measurements.Where(x => x.Status == MeasurementStatus.Success && x.Timestamp >= currentDate - TimeSpan.FromSeconds(IntervalMs/1000)).ToList();
        _logger.LogInformation("Creating snapshot");
        if (measurements.IsNullOrEmpty())
        {
            _logger.LogWarning("Brak pomiarów do stworzenia snapshotu");
            return null;
        }

        var measurementSnapshot = new MeasurementSnapshot
        {
            Timestamp = DateTimeOffset.UtcNow,
            Temperature = measurements.Average(x => x.Temperature)!.Value,
            Humidity = measurements.Average(x => x.Humidity)!.Value,
        };

        await context.MeasurementSnapshots.AddAsync(measurementSnapshot, ct);
        await context.SaveChangesAsync(ct);

        return measurementSnapshot;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        while (!ct.IsCancellationRequested)
        {
            var measurementSnapshot = await CreateSnapshotAsync(context, ct);
            if (measurementSnapshot != null)
            {
                await _channel.Writer.WriteAsync(measurementSnapshot, ct);
            }
            await Task.Delay(IntervalMs, ct);
        }
    }
}