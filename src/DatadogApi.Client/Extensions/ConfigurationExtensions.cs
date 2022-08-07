using DatadogApi.Client.Settings;
using Microsoft.Extensions.Configuration;

namespace DatadogApi.Client.Extensions
{
    public static class ConfigurationExtensions
    {
        public static DatadogApiSettings GetDatadogApiSettings(this IConfiguration configuration) =>
            new DatadogApiSettings(
                configuration.GetSection("DatadogApiSettings:ApiKey").Value,
                configuration.GetSection("DatadogApiSettings:ApplicationKey").Value);
    }
}