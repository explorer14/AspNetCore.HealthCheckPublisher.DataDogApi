using DatadogApi.Client.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DatadogApi.Client
{
    public interface IApplicationHealthReporter
    {
        Task SendHealthReport(HealthReport healthReport, HealthReportOptions healthReportOptions);
    }
}