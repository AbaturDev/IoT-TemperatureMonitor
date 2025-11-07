namespace TemperatureMonitor.Database.Entities;

public sealed record MeasurementSnapshot
{
    public Guid Id { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public double Temperature { get; init; }
    public double Humidity { get; init; }
}