namespace DatadogApi.Client.Settings
{
    public class HealthReportOptions
    {
        internal HealthReportOptions(string applicationPrefix) =>
            ApplicationPrefix = applicationPrefix;

        /// <summary>
        ///   This prefix will be added to the health metrics your application
        ///   publishes using this package for e.g. order-api.health....
        /// </summary>
        public string ApplicationPrefix { get; } = string.Empty;

        /// <summary>
        ///   Any optional tags you always want this package to push to Datadog
        ///   with your health telemetry like current execution environment etc
        /// </summary>
        internal Dictionary<string, string> DefaultMetricTags { get; set; }
            = new Dictionary<string, string>();
    }
}