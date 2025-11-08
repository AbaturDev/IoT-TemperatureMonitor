using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using TemperatureMonitor.BackgroundServices;
using TemperatureMonitor.Database;
using TemperatureMonitor.Database.Entities;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHostedService<MqttBackgroundService>();
builder.Services.AddHostedService<SnapshotBackgroundService>();

builder.Services.AddOpenApi();

builder.Services.AddSingleton(Channel.CreateBounded<MeasurementSnapshot>(
    new BoundedChannelOptions(1)
    {
        SingleWriter = true,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest
    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("frontend");

app.UseHttpsRedirection();

app.MapMeasurementSnapshotEndpoint();

app.Run();
