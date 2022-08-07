using DatadogApi.Client;
using DatadogApi.Client.Extensions;
using DatadogApi.Client.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

builder.Services.AddHealthChecks()
    .AddCheck<ServiceLivenessCheck>("Liveness")
    .AddDatadogPublisher(
        builder.Configuration.GetDatadogApiSettings(),
        new HealthReportOptions
        {
            ApplicationPrefix = "myapi",
            DefaultMetricTags = new Dictionary<string, string>(new[]
            {
                new KeyValuePair<string, string>("environment", builder.Environment.EnvironmentName)
            })
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHealthChecks("/health/status");

app.MapControllers();

app.Run();

internal class ServiceLivenessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy());
}