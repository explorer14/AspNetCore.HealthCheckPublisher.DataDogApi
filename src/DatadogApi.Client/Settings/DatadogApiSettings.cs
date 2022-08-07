namespace DatadogApi.Client.Settings
{
    public class DatadogApiSettings
    {
        public DatadogApiSettings(
            string? apiKey,
            string? applicationKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(
                    nameof(apiKey),
                    "Datadog API key is null or missing in application configuration");

            if (string.IsNullOrWhiteSpace(applicationKey))
                throw new ArgumentNullException(
                    nameof(applicationKey),
                    "Datadog application key is null or missing in application configuration");

            ApiKey = apiKey;
            ApplicationKey = applicationKey;
        }

        public string ApiKey { get; init; }
        public string ApplicationKey { get; init; }
    }
}