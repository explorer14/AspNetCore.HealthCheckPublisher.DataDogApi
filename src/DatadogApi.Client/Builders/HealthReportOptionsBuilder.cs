using DatadogApi.Client.Settings;

namespace DatadogApi.Client.Builders
{
    public class HealthReportOptionsBuilder
    {
        private HealthReportOptions healthReportOptions;

        public HealthReportOptionsBuilder(string applicationPrefix) =>
            healthReportOptions = new HealthReportOptions(applicationPrefix);

        public HealthReportOptionsBuilder WithOptionalDefaultMetricTag(
            string tagKey, string tagValue)
        {
            healthReportOptions.DefaultMetricTags.Add(tagKey, tagValue);
            return this;
        }

        public HealthReportOptions Build() =>
            healthReportOptions;
    }
}