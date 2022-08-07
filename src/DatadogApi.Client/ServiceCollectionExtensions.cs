using Microsoft.Extensions.DependencyInjection;

namespace DatadogApi.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddDatadogHealthReporter(
            this IServiceCollection services,
            DatadogApiSettings datadogApiSettings) =>
            services.AddSingleton(datadogApiSettings)
                .AddHttpClient<IApplicationHealthReporter, DatadogHealthReporter>()
                .ConfigureHttpClient(client =>
                    client.BaseAddress = new Uri("https://app.datadoghq.com"));
    }
}