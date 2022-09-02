using AspNetCore.HealthCheckPublisher.DataDogApi.Builders;
using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Tests.TestData
{
    internal class DefaultHealthReportOptions
    {
        internal static HealthReportOptions Instance =>
            new HealthReportOptionsBuilder(applicationPrefix: "TESTAPP")
                .WithOptionalDefaultMetricTag("Environment", "testing")
                .Build();
    }
}