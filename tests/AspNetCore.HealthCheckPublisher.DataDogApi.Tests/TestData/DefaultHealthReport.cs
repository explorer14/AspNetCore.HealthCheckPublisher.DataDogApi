using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Tests.TestData
{
    internal class DefaultHealthReport
    {
        internal static HealthReport InstanceWithHealthyResult => new HealthReport(
            new Dictionary<string, HealthReportEntry>(
                new KeyValuePair<string, HealthReportEntry>[]
                {
                    new("TEST",new HealthReportEntry())
                }),
            HealthStatus.Healthy,
            TimeSpan.Zero);

        internal static HealthReport InstanceWithNoDependencies => new HealthReport(
                    new Dictionary<string, HealthReportEntry>(),
                    HealthStatus.Healthy,
                    TimeSpan.Zero);
    }
}