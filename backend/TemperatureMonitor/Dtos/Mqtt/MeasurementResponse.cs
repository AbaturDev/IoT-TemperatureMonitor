using System.Text.Json.Serialization;
using TemperatureMonitor.Database.Enums;

namespace TemperatureMonitor.Dtos.Mqtt;

public sealed record MeasurementResponse
{
    [JsonPropertyName("status")]
    public required MeasurementStatus Status { get; init; }
    
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }
    
    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }
    
    [JsonPropertyName("humidity")]
    public double? Humidity { get; init; }
    
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}