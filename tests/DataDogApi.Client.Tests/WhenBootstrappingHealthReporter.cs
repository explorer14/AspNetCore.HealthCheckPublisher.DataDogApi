using DatadogApi.Client;
using DatadogApi.Client.Extensions;
using DatadogApi.Client.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DataDogApi.Client.Tests
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