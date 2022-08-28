using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Extensions
{
    public static class HealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddDatadogPublisher(
            this IHealthChecksBuilder healthChecksBuilder,
            DatadogApiSettings datadogApiSettings,
            HealthReportOptions healthReportOptions)
        {
            healthChecksBuilder.Services.Configure<HealthCheckPublisherOptions>(hcpo =>
                hcpo.Period = TimeSpan.FromMinutes(1));
            healthChecksBuilder.Services.AddDatadogHealthReporter(datadogApiSettings);

            healthChecksBuilder.Services.AddSingleton<IHealthCheckPublisher>(sp =>
                new ApplicationHealthCheckPublisher(
                    sp.GetService<IApplicationHealthReporter>()!,
                    healthReportOptions));

            return healthChecksBuilder;
        }
    }
}