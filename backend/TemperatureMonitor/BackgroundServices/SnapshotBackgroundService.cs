using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
using TemperatureMonitor.Database.Enums;
using TemperatureMonitor.Dtos.MeasurementSnapshots;

namespace TemperatureMonitor.BackgroundServices;

public class SnapshotBackgroundService : BackgroundService
{
    private readonly ILogger<SnapshotBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<MeasurementSnapshotDto> _channel;
    private readonly TimeProvider _timeProvider;
    
    private const int IntervalMs = 300_000; // 5 min
    
    public SnapshotBackgroundService(ILogger<SnapshotBackgroundService> logger, IServiceProvider serviceProvider, Channel<MeasurementSnapshotDto> channel, TimeProvider timeProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _channel = channel;
        _timeProvider = timeProvider;
    }

    private async Task<MeasurementSnapshot?> CreateSnapshotAsync(AppDbContext context, CancellationToken ct)
    {
        var currentDate = _timeProvider.GetUtcNow();
        
        var query = context.Measurements
            .Where(x => x.Status == MeasurementStatus.Success &&
                        x.Timestamp >= currentDate - TimeSpan.FromSeconds(IntervalMs / 1000));

        var count = await query.CountAsync(ct);
        
        var measurements = query.ToList();
        
        if (count == 0)
        {
            _logger.LogWarning("No measurements to create snapshot");
            return null;
        }
        _logger.LogInformation("Creating snapshot");

        var measurementSnapshot = new MeasurementSnapshot
        {
            Timestamp = currentDate,
            TemperatureAvg = await query.AverageAsync(x => x.Temperature!.Value, ct),
            TemperatureMax = await query.MaxAsync(x => x.Temperature!.Value, ct),
            TemperatureMin = await query.MinAsync(x => x.Temperature!.Value, ct),
            HumidityAvg = await query.AverageAsync(x => x.Humidity!.Value, ct),
            Count = count,
            Measurements = measurements
        };

        await context.MeasurementSnapshots.AddAsync(measurementSnapshot, ct);
        await context.SaveChangesAsync(ct);

        return measurementSnapshot;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var measurementSnapshot = await CreateSnapshotAsync(context, ct);
                if (measurementSnapshot != null)
                {
                    var dto = new MeasurementSnapshotDto
                    {
                        Id = measurementSnapshot.Id,
                        Timestamp = measurementSnapshot.Timestamp,
                        TemperatureAvg = measurementSnapshot.TemperatureAvg,
                        TemperatureMin = measurementSnapshot.TemperatureMin,
                        TemperatureMax = measurementSnapshot.TemperatureMax,
                        HumidityAvg = measurementSnapshot.HumidityAvg,
                        Count = measurementSnapshot.Count
                    };
                    
                    await _channel.Writer.WriteAsync(dto, ct);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while creating measurement snapshot");
            }

            await Task.Delay(IntervalMs, ct);
        }
    }
}