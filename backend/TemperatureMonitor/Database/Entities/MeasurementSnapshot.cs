namespace TemperatureMonitor.Database.Entities;

public record MeasurementSnapshot
{
    public Guid Id { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public double TemperatureAvg { get; init; }
    public double HumidityAvg { get; init; }
    public double TemperatureMin { get; init; }
    public double TemperatureMax { get; init; }
    public int Count { get; init; }
    public List<Measurement> Measurements { get; init; } = new();
}