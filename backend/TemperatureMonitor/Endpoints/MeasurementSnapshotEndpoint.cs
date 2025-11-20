using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
using TemperatureMonitor.Database.Enums;
using TemperatureMonitor.Dtos.MeasurementSnapshots;
using TemperatureMonitor.Services;

namespace TemperatureMonitor.Endpoints;

public static class MeasurementSnapshotEndpoint
{
    public static void MapMeasurementSnapshotEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Measurements")
            .AllowAnonymous()
            .WithOpenApi();
        
        group.MapGet("/live/measurements", async (HttpResponse response, [FromServices] Channel<MeasurementSnapshotDto> channel, CancellationToken ct) =>
        {
            response.Headers.Append("Content-Type", "text/event-stream");
            response.Headers.Append("Cache-Control", "no-cache");

            await foreach (var measurementSnapshot in channel.Reader.ReadAllAsync(ct))
            {
                var json = JsonSerializer.Serialize(measurementSnapshot);
                await response.WriteAsync($"data: {json}\n\n", ct);
                await response.Body.FlushAsync(ct);
            }

            _ = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    await response.WriteAsync(":\n\n", ct);
                    await response.Body.FlushAsync(ct);
                    await Task.Delay(TimeSpan.FromSeconds(30), ct);
                }
            }, ct);
        })
        .Produces<MeasurementSnapshotDto>(StatusCodes.Status200OK, "text/event-stream")
        .WithDescription("Returns a continuous stream of live measurement snapshots via Server-Sent Events");

        group.MapGet("/measurements/latest", async (AppDbContext context, CancellationToken ct) =>
        {
            var maxTimestamp = await context.MeasurementSnapshots.MaxAsync(x => x.Timestamp, ct);
            
            var measurementSnapshot = await context.MeasurementSnapshots.FirstOrDefaultAsync(x => x.Timestamp == maxTimestamp, ct);
            if (measurementSnapshot == null)
            {
                return Results.NotFound();
            }
            
            var result = new MeasurementSnapshotDto
            {
                Id = measurementSnapshot.Id,
                Timestamp = measurementSnapshot.Timestamp,
                TemperatureAvg = measurementSnapshot.TemperatureAvg,
                TemperatureMin = measurementSnapshot.TemperatureMin,
                TemperatureMax = measurementSnapshot.TemperatureMax,
                HumidityAvg = measurementSnapshot.HumidityAvg,
                Count = measurementSnapshot.Count
            };
        
            return Results.Ok(result);
        })
        .Produces<MeasurementSnapshotDto>(StatusCodes.Status200OK, "application/json")
        .WithDescription("Returns a latest measurement snapshot");
        
        group.MapGet("/measurements", async (AppDbContext context, [FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate, CancellationToken ct) =>
            {
                var snapshots = await context.MeasurementSnapshots
                    .Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate)
                    .OrderBy(x => x.Timestamp)
                    .ToListAsync(ct);

                if (!snapshots.Any())
                    return Results.NotFound(new { message = "No measurements found in the given period" });

                var temps = snapshots.Select(s => s.TemperatureMin).ToArray(); // temperatureMax
                var trend = TrendCalculator.CalculateLinearTrend(temps);

                var result = snapshots.Select((s, i) => new MeasurementSnapshotWithTrendDto
                    {
                        Id = s.Id,
                        Timestamp = s.Timestamp,
                        TemperatureAvg = s.TemperatureAvg,
                        TemperatureMin = s.TemperatureMin,
                        TemperatureMax = s.TemperatureMax,
                        HumidityAvg = s.HumidityAvg,
                        Count = s.Count,
                        TemperatureTrend = trend[i]
                    }).OrderByDescending(x => x.Timestamp)
                    .ToList();

                return Results.Ok(result);
            })
            .Produces<List<MeasurementSnapshotWithTrendDto>>(StatusCodes.Status200OK, "application/json")
            .WithDescription("Returns measurement snapshots for given time period with trend");

        group.MapGet("/sensor/latest", async (AppDbContext context, CancellationToken ct) =>
        {
            var maxTimestamp = await context.Measurements.MaxAsync(x => x.Timestamp, ct);
            
            var result = await context.Measurements.FirstOrDefaultAsync(x => x.Timestamp == maxTimestamp, ct);

            return Results.Ok(result);
        })
        .Produces<MeasurementStatus>(StatusCodes.Status200OK, "application/json")
        .WithDescription("Returns last response from sensor");
    }

}