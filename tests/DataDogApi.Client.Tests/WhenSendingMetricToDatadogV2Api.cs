using DatadogApi.Client;
using DatadogApi.Client.Extensions;
using DatadogApi.Client.Metrics;
using DatadogApi.Client.Settings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DataDogApi.Client.Tests
{
    public class WhenSendingMetricToDatadogV2Api
    {
        [Fact]
        public async Task ShouldIncludeHeadersInRequest()
        {
            const string expectedApiKey = "APIKEY";
            const string expectedApplicationKey = "APPKEY";

            var apiRequestSpy = new DatadogApiRequestSpy();
            var ddReporter = new DatadogHealthReporter(
                new HttpClient(apiRequestSpy)
                {
                    BaseAddress = new Uri("https://foo.bar")
                },
                new DatadogApiSettings(
                    expectedApiKey,
                    expectedApplicationKey));

            await ddReporter.SendHealthReport(
                new HealthReport(
                    new Dictionary<string, HealthReportEntry>(
                        new KeyValuePair<string, HealthReportEntry>[]
                        {
                            new("TEST",new HealthReportEntry())
                        }), HealthStatus.Healthy, TimeSpan.Zero),
                new HealthReportOptions(applicationPrefix: "TESTAPP")
                {
                    DefaultMetricTags = new Dictionary<string, string>(
                        new KeyValuePair<string, string>[]
                        {
                            new("Environment", "testing")
                        })
                });

            apiRequestSpy.Headers.Should()
                .Satisfy(apiKeyHeader => apiKeyHeader.Key == "DD-API-KEY" &&
                                         apiKeyHeader.Value.Single() == expectedApiKey,
                        appKeyHeader => appKeyHeader.Key == "DD-APPLICATION-KEY" &&
                                        appKeyHeader.Value.Single() == expectedApplicationKey);
        }

        [Fact]
        public async Task ShouldSendCompliantRequestBody()
        {
            var metric = JsonConvert.DeserializeObject<CountMetric>(
                "{\r\n    \"series\": [\r\n        {\r\n            \"metric\": \"TESTAPP.app.ishealthy\",\r\n            \"type\": 1,\r\n            \"points\": [\r\n                {\r\n                    \"timestamp\": 0,\r\n                    \"value\": 2\r\n                }\r\n            ],\r\n            \"tags\": [\r\n                \"Environment:testing\"\r\n            ],\r\n            \"resources\": [\r\n                {\r\n                    \"name\": \"machine1\",\r\n                    \"type\": \"host\"\r\n                }\r\n            ]\r\n        }\r\n    ]\r\n}");

            var apiRequestSpy = new DatadogApiRequestSpy();
            var ddReporter = new DatadogHealthReporter(
                new HttpClient(apiRequestSpy)
                {
                    BaseAddress = new Uri("https://foo.bar")
                },
                new DatadogApiSettings(
                    "APIKEY",
                    "APPKEY"));

            await ddReporter.SendHealthReport(
                new HealthReport(
                    new Dictionary<string, HealthReportEntry>(),
                    HealthStatus.Healthy,
                    TimeSpan.Zero),
                new HealthReportOptions(applicationPrefix: "TESTAPP")
                {
                    DefaultMetricTags = new Dictionary<string, string>(
                        new KeyValuePair<string, string>[]
                        {
                            new("Environment", "testing")
                        })
                });

            apiRequestSpy.RequestBodyAsMetric.Should().BeEquivalentTo(metric);
        }

        [Fact]
        public async Task ShouldSetCorrectBaseAddress()
        {
            var svcs = new ServiceCollection();
            var apiRequestSpy = new DatadogApiRequestSpy();

            svcs.AddDatadogHealthReporter(new DatadogApiSettings("FAKE", "FAKE"))
                .ConfigurePrimaryHttpMessageHandler(_ => apiRequestSpy);

            using (var scope = svcs.BuildServiceProvider().CreateScope())
            {
                var reporter = scope.ServiceProvider.GetService<IApplicationHealthReporter>();
                await reporter.SendHealthReport(
                new HealthReport(
                    new Dictionary<string, HealthReportEntry>(
                        new KeyValuePair<string, HealthReportEntry>[]
                        {
                            new("TEST",new HealthReportEntry())
                        }), HealthStatus.Healthy, TimeSpan.Zero),
                new HealthReportOptions(applicationPrefix: "TESTAPP")
                {
                    DefaultMetricTags = new Dictionary<string, string>(
                        new KeyValuePair<string, string>[]
                        {
                            new("Environment", "testing")
                        })
                });

                apiRequestSpy.HostAddress.Should().Be("https://app.datadoghq.com");
            }
        }
    }

    internal class DatadogApiRequestSpy : HttpMessageHandler
    {
        internal HttpRequestHeaders Headers { get; private set; }

        internal string HostAddress { get; private set; }
        internal CountMetric RequestBodyAsMetric { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Headers = request.Headers;
            RequestBodyAsMetric = JsonConvert.DeserializeObject<CountMetric>(
                request.Content?.ReadAsStringAsync(cancellationToken)
                    .GetAwaiter().GetResult() ?? string.Empty)!;
            RequestBodyAsMetric.series.First().points.First().timestamp = 0;
            RequestBodyAsMetric.series.First().resources.First().name = "machine1";
            HostAddress = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";

            return Task.FromResult(
                new HttpResponseMessage(
                    System.Net.HttpStatusCode.OK));
        }
    }
}