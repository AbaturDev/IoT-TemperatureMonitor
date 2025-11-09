using System.Text.Json.Serialization;

namespace TemperatureMonitor.Dtos.MeasurementSnapshots;

public sealed record MeasurementSnapshotDto
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }
    
    [JsonPropertyName("temperatureAvg")]
    public required double TemperatureAvg { get; init; }
    
    [JsonPropertyName("temperatureMin")]
    public required double TemperatureMin { get; init; }
    
    [JsonPropertyName("temperatureMax")]
    public required double TemperatureMax { get; init; }
    
    [JsonPropertyName("humidity")]
    public required double HumidityAvg { get; init; }
    
    [JsonPropertyName("count")]
    public required int Count { get; init; }
}