using TemperatureMonitor.Database.Enums;

namespace TemperatureMonitor.Database.Entities;

public sealed record Measurement
{
    public Guid Id { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public MeasurementStatus Status { get; init; }
    public double? Temperature { get; init; }
    public double? Humidity { get; init; }
    public string? Message { get; init; }
}