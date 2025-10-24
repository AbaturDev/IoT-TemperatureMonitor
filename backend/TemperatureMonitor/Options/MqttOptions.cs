namespace TemperatureMonitor.Options;

public class MqttOptions
{
    public const string SectionName = "MQTT";

    public required string BrokerAddress { get; init; }
    public required int BrokerPort { get; init; }
    public required string ClientId { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
    public required string Subscription { get; init; }
}