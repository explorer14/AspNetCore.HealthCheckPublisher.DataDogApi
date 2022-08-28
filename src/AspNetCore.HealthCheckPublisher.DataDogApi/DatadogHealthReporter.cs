using AspNetCore.HealthCheckPublisher.DataDogApi.Metrics;
using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace AspNetCore.HealthCheckPublisher.DataDogApi
{
    public class DatadogHealthReporter : IApplicationHealthReporter
    {
        private readonly DatadogApiSettings datadogApiSettings;
        private readonly HttpClient httpClient;

        public DatadogHealthReporter(
            HttpClient httpClient,
            DatadogApiSettings datadogApiSettings)
        {
            this.httpClient = httpClient;
            this.datadogApiSettings = datadogApiSettings;
        }

        public async Task SendHealthReport(
            HealthReport healthReport,
            HealthReportOptions healthReportOptions)
        {
            var metricTags =
                healthReportOptions.DefaultMetricTags
                    .Select(x => $"{x.Key}:{x.Value}")
                    .ToList();

            var posixTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await SendMetricForOverallAppHealth(
                healthReport,
                healthReportOptions.ApplicationPrefix,
                metricTags, posixTimeStamp);

            foreach (var item in healthReport.Entries)
            {
                await SendMetricForComponentHealth(
                    healthReportOptions.ApplicationPrefix,
                    metricTags, posixTimeStamp, item);
            }
        }

        private async Task Post(string json)
        {
            var ddApiKey = datadogApiSettings.ApiKey;
            var ddAppKey = datadogApiSettings.ApplicationKey;
            var jsonContent = new StringContent(json,
                    Encoding.UTF8, "application/json");
            Log.Logger.Information($"JSON was {json}");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/v2/series")
            {
                Content = jsonContent,
            };

            request.Headers.TryAddWithoutValidation("DD-API-KEY", ddApiKey);
            request.Headers.TryAddWithoutValidation("DD-APPLICATION-KEY", ddAppKey);

            var ddResponse = await httpClient.SendAsync(request);
            ddResponse.EnsureSuccessStatusCode();
            var ddResponseContent = await ddResponse.Content.ReadAsStringAsync();
            Log.Logger.Information($"DD returned {ddResponseContent}");
        }

        private async Task SendGuageMetric(GuageMetric guageMetric)
        {
            var json = JsonConvert.SerializeObject(
                guageMetric,
                Formatting.Indented);

            await Post(json);
        }

        private async Task SendMetric(CountMetric countMetric)
        {
            var json = JsonConvert.SerializeObject(
                countMetric,
                Formatting.Indented);

            await Post(json);
        }

        private async Task SendMetricForComponentHealth(
            string metricAppPrefix,
            List<string> metricTags,
            long posixTimeStamp,
            KeyValuePair<string, HealthReportEntry> item)
        {
            Log.Logger.Information(
                "Health test :{test},result: {result}",
                item.Key,
                item.Value.Status);

            CountMetric countMetric = new CountMetric(
                metricName: $"{metricAppPrefix}.app." +
                    $"{item.Key.ToLower().Replace(" ", "_")}.ishealthy",
                posixTimeStamp: posixTimeStamp,
                count: (int)item.Value.Status,
                tags: metricTags.ToList());

            GuageMetric guageMetric = new GuageMetric(
                metricName: $"{metricAppPrefix}.app." +
                    $"{item.Key.ToLower().Replace(" ", "_")}.duration",
                posixTimeStamp: posixTimeStamp,
                value: (int)item.Value.Duration.TotalMilliseconds,
                tags: metricTags);

            await SendMetric(countMetric);
            await SendGuageMetric(guageMetric);
        }

        private async Task SendMetricForOverallAppHealth(
            HealthReport healthReport,
            string metricAppPrefix,
            List<string> metricTags,
            long posixTimeStamp)
        {
            Log.Logger.Information(
                "Overall health result: {result}",
                healthReport.Status);

            CountMetric countMetric1 = new CountMetric(
                    metricName: $"{metricAppPrefix}.app.ishealthy",
                    posixTimeStamp: posixTimeStamp,
                    count: (int)healthReport.Status,
                    tags: metricTags.ToList());

            await SendMetric(countMetric1);
        }
    }
}