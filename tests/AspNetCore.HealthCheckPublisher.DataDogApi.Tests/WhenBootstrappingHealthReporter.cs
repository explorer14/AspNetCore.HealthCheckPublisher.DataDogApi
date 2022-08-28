using AspNetCore.HealthCheckPublisher.DataDogApi;
using AspNetCore.HealthCheckPublisher.DataDogApi.Extensions;
using AspNetCore.HealthCheckPublisher.DataDogApi.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.HealthCheckPublisher.DataDogApi.Tests
{
    public class WhenBootstrappingHealthReporter
    {
        [Fact]
        public void ShouldResolveANonNullInstanceFromDI()
        {
            var svcs = new ServiceCollection();

            svcs.AddDatadogHealthReporter(new DatadogApiSettings("FAKE", "FAKE"));

            using (var scope = svcs.BuildServiceProvider().CreateScope())
            {
                var reporter = scope.ServiceProvider.GetService<IApplicationHealthReporter>();
                Assert.NotNull(reporter);
            }
        }
    }
}