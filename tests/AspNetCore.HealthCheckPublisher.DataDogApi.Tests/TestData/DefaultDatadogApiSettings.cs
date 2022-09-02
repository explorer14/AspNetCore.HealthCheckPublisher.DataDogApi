using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Tests.TestData
{
    internal class DefaultDatadogApiSettings
    {
        internal static DatadogApiSettings Instance =>
            new DatadogApiSettings("FAKE", "FAKE");
    }
}