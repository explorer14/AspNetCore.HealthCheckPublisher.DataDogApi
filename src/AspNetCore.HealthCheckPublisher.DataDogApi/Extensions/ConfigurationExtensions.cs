using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Extensions
{
    public static class ConfigurationExtensions
    {
        public static DatadogApiSettings GetDatadogApiSettings(this IConfiguration configuration) =>
            new DatadogApiSettings(
                configuration.GetSection("DatadogApiSettings:ApiKey").Value,
                configuration.GetSection("DatadogApiSettings:ApplicationKey").Value);
    }
}