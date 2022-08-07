# Intro

Package to publish results of an ASP.NET Core health check to Datadog. Here's how to use it

### Step 1: Add your custom health checks to IServiceCollection DI

```csharp
services.AddHealthChecks()
        .AddCheck<MyAppHealthCheck>("MyAppHealthCheck");        
```

### Step 2: Make sure you have the Datadog API key and application key and the following settings exist in your `secrets.json` and your CI environment (or you have some secure way of providing these into your application)

```javascript
{
  "DatadogApiSettings": {
    "ApiKey": "<DATADOG API KEY>",
    "ApplicationKey": "<APPLICATION KEY THAT THE API KEY WILL WORK WITH>"
  }
}
```

### Step 3: Add this package (`DatadogApi.Client`) to the `IHealthChecksBuilder` chain

```csharp
services
        .AddHealthChecks()
        .AddCheck<MyAppHealthCheck>("MyAppHealthCheck")
        .AddDatadogPublisher(
            Configuration.GetDatadogApiSettings(),
            new HealthReportOptions
            {
                ApplicationPrefix = "traxpense",
                DefaultMetricTags = new Dictionary<string, string>(new[]
                {
                    new KeyValuePair<string, string>("Environment",
                        Environment.EnvironmentName.ToLowerInvariant())
                })
            });
```

### Step 4: Modify your Serilog configuration to filter out events with sensitive data:

```csharp
Log.Logger = new LoggerConfiguration()
                ...                
                .FilterOutEventsWithSensitiveInfo()
                ...
```

The reason for this is that the package uses Serilog, if your application also uses Serilog, then you'd want to scrub the API key and application key that get logged by the HTTP pipeline that the publisher uses. Currently there is no way to customise the pipeline to scrub the sensitive data so you will have to add this line to your Serilog builder chain.

If your application doesn't use Serilog, then you don't need this setting.
