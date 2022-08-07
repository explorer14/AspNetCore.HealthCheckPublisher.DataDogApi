namespace DatadogApi.Client.Settings
{
    public class HealthReportOptions
    {
        public string ApplicationPrefix { get; set; } = string.Empty;

        public Dictionary<string, string> DefaultMetricTags { get; set; }
            = new Dictionary<string, string>();
    }
}