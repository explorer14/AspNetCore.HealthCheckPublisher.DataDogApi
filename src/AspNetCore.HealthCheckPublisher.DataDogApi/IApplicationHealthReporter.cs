using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthCheckPublisher.DataDogApi
{
    public interface IApplicationHealthReporter
    {
        Task SendHealthReport(
            HealthReport healthReport,
            HealthReportOptions? healthReportOptions);
    }
}