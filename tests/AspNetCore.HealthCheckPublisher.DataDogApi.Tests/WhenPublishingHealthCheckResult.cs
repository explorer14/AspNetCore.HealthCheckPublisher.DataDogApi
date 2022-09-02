using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using AspNetCore.HealthCheckPublisher.DataDogApi.Tests.TestData;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Tests
{
    public class WhenPublishingHealthCheckResult
    {
        [Fact]
        public async Task ShouldInvokeApplicationHealthReporter()
        {
            var expectedHealthReport = DefaultHealthReport.InstanceWithHealthyResult;

            var spyHealthReporter = new SpyHealthReporter();
            var publisher = new ApplicationHealthCheckPublisher(
                spyHealthReporter, DefaultHealthReportOptions.Instance);

            await publisher.PublishAsync(expectedHealthReport, CancellationToken.None);

            spyHealthReporter.WasInvoked.Should().BeTrue();
            spyHealthReporter.PublishedReport.Should().BeEquivalentTo(expectedHealthReport);
        }
    }

    internal class SpyHealthReporter : IApplicationHealthReporter
    {
        public bool WasInvoked { get; private set; }
        public HealthReport? PublishedReport { get; private set; }

        public Task SendHealthReport(HealthReport healthReport, HealthReportOptions healthReportOptions)
        {
            WasInvoked = true;
            PublishedReport = healthReport;
            return Task.CompletedTask;
        }
    }
}