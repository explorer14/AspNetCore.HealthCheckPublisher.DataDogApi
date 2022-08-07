using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DatadogApi.Client
{
    public interface IApplicationHealthReporter
    {
        Task SendHealthReport(HealthReport healthReport,
            string applicationPrefix,
            Dictionary<string, string> metricTags);
    }
}