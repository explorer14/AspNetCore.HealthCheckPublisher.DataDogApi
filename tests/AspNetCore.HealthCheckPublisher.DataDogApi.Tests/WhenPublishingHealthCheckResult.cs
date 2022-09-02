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

        [Fact]
        public async Task ShouldThrowIfHealthReportIsNull()
        {
            var dummyHealthReporter = new DatadogHealthReporter(
                new HttpClient(new DummyHttpMessageHandler())
                {
                    BaseAddress = new Uri("http://foo.bar")
                }, DefaultDatadogApiSettings.Instance);

            var publisher = new ApplicationHealthCheckPublisher(
                dummyHealthReporter, DefaultHealthReportOptions.Instance);

            await this.Invoking(_ =>
                publisher.PublishAsync(null!, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }
    }

    internal class DummyHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(
                new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK));
    }

    internal class SpyHealthReporter : IApplicationHealthReporter
    {
        public HealthReport? PublishedReport { get; private set; }
        public bool WasInvoked { get; private set; }

        public Task SendHealthReport(
            HealthReport healthReport,
            HealthReportOptions? healthReportOptions)
        {
            WasInvoked = true;
            PublishedReport = healthReport;
            return Task.CompletedTask;
        }
    }
}