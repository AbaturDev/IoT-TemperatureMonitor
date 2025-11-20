namespace TemperatureMonitor.Database.Entities;

public sealed record MeasurementSnapshotWithTrend : MeasurementSnapshot
{
    public double TemperatureTrend { get; init; }
}