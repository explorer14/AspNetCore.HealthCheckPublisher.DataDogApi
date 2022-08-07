using DatadogApi.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DataDogApi.Client.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
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