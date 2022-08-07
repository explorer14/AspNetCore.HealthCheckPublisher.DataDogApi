using DatadogApi.Client.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DatadogApi.Client
{
    public class ApplicationHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IApplicationHealthReporter applicationHealthReporter;
        private readonly HealthReportOptions healthReportOptions;

        public ApplicationHealthCheckPublisher(
            IApplicationHealthReporter applicationHealthReporter,
            HealthReportOptions healthReportOptions)
        {
            this.applicationHealthReporter = applicationHealthReporter;
            this.healthReportOptions = healthReportOptions;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken) =>
            applicationHealthReporter.SendHealthReport(report, healthReportOptions);
    }
}