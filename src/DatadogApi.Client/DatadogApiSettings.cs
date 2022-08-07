namespace DatadogApi.Client
{
    public class DatadogApiSettings
    {
        public DatadogApiSettings(
            string apiKey,
            string applicationKey)
        {
            ApiKey = apiKey;
            ApplicationKey = applicationKey;
        }

        public string ApiKey { get; init; }
        public string ApplicationKey { get; init; }
    }
}