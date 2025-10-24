using System.Text.Json;
using Microsoft.Extensions.Options;
using MQTTnet;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
using TemperatureMonitor.Dtos.Mqtt;
using TemperatureMonitor.Options;

namespace TemperatureMonitor.BackgroundServices;

public class MqttBackgroundService : BackgroundService
{
    private readonly ILogger<MqttBackgroundService> _logger;
    private readonly IOptions<MqttOptions> _options;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly IServiceProvider _serviceProvider;
    
    public MqttBackgroundService(ILogger<MqttBackgroundService> logger, IOptions<MqttOptions> options, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options;
        _serviceProvider = serviceProvider;

        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();
    
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(options.Value.BrokerAddress, options.Value.BrokerPort)
            .WithCredentials(options.Value.User, options.Value.Password)
            .WithClientId(options.Value.ClientId)
            .WithCleanSession()
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            _logger.LogInformation("MQTT Message Received");
            try
            {
                var content = e.ApplicationMessage.ConvertPayloadToString();
                var response = JsonSerializer.Deserialize<MeasurementResponse>(content);

                if (response is null)
                {
                    _logger.LogWarning("MQTT Message content was not measurement");
                    return;
                }
                
                var measurement = new Measurement
                {
                    Status = response.Status,
                    Timestamp = response.Timestamp,
                    Temperature = response.Temperature,
                    Humidity = response.Humidity,
                    Message = response.Message,
                };

                context.Measurements.Add(measurement);
                await context.SaveChangesAsync(stoppingToken);
            }
            catch
            {
                _logger.LogWarning("Failed to process MQTT Message as measurement");
            }
        };

        _mqttClient.DisconnectedAsync += async _ =>
        {
            _logger.LogWarning("Disconnected from MQTT broker. Reconnecting...");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                await _mqttClient.SubscribeAsync(_options.Value.Subscription, cancellationToken: stoppingToken);
                _logger.LogInformation("Reconnected to MQTT broker.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reconnect.");
            }
        };
        
        await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
        await _mqttClient.SubscribeAsync(_options.Value.Subscription, cancellationToken: stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}