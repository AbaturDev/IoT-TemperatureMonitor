using TemperatureMonitor.Database.Enums;

namespace TemperatureMonitor.Dtos;

public sealed record StatusDto
{
    public DateTimeOffset Timestamp { get; init; }
    public MeasurementStatus Status { get; init; }
}