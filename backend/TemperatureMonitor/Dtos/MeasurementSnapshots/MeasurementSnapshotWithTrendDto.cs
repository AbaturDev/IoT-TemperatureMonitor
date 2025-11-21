using System.Text.Json.Serialization;

namespace TemperatureMonitor.Dtos.MeasurementSnapshots;

public sealed record MeasurementSnapshotWithTrendDto : MeasurementSnapshotDto
{
    [JsonPropertyName("temperatureTrend")]
    public required double TemperatureTrend { get; init; }
    
    [JsonPropertyName("temperatureStdDev")]
    public double TemperatureStdDev { get; init; } 
}