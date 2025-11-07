using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.BackgroundServices;
using TemperatureMonitor.Database;
using TemperatureMonitor.Endpoints;
using TemperatureMonitor.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);

builder.Services.AddOptions<MqttOptions>()
    .BindConfiguration(MqttOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHostedService<MqttBackgroundService>();
builder.Services.AddHostedService<SnapshotBackgroundService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMeasurementEndpoint();

app.Run();
