using DatadogApi.Client.Metrics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace DatadogApi.Client
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
            string applicationPrefix,
            Dictionary<string, string> metricTags)
        {
            var metricTagsString = string.Join(',', metricTags.Select(x => $"{x.Key}:{x.Value}"));
            var posixTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await SendMetricForOverallAppHealth(
                healthReport,
                applicationPrefix,
                metricTagsString, posixTimeStamp);

            foreach (var item in healthReport.Entries)
            {
                await SendMetricForComponentHealth(
                    applicationPrefix,
                    metricTagsString, posixTimeStamp, item);
            }
        }

        private async Task Post(string json)
        {
            var ddApiKey = datadogApiSettings.ApiKey;
            var ddAppKey = datadogApiSettings.ApplicationKey;
            var jsonContent = new StringContent(json,
                    Encoding.UTF8, "application/json");
            Log.Logger.Information($"JSON was {json}");

            var ddResponse = await httpClient.PostAsync(
                $"/api/v1/series?api_key={ddApiKey}&application_key={ddAppKey}",
                jsonContent);
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
            string metricTags,
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
                tags: metricTags);

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
            string metricTags,
            long posixTimeStamp)
        {
            Log.Logger.Information(
                "Overall health result: {result}",
                healthReport.Status);

            CountMetric countMetric1 = new CountMetric(
                    metricName: $"{metricAppPrefix}.app.ishealthy",
                    posixTimeStamp: posixTimeStamp,
                    count: (int)healthReport.Status,
                    tags: metricTags);

            await SendMetric(countMetric1);
        }
    }
}