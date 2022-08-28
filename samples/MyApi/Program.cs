using AspNetCore.HealthCheckPublisher.DataDogApi.Builders;
using AspNetCore.HealthCheckPublisher.DataDogApi.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}]{Properties:j}{Message:lj}{NewLine}{Exception}")
    .Enrich
    .FromLogContext()
    .CreateLogger();

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
        new HealthReportOptionsBuilder(applicationPrefix: "myapi")
        .WithOptionalDefaultMetricTag("environment", builder.Environment.EnvironmentName)
        .Build());

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