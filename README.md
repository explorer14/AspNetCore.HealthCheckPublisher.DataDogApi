# Intro
[![](https://img.shields.io/badge/Nuget-v3.0.0-green?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/AspNetCore.HealthCheckPublisher.DataDogApi)

[![Build Status](https://skynetcode.visualstudio.com/Libraries/_apis/build/status/explorer14.AspNetCore.HealthCheckPublisher.DataDogApi?branchName=master)](https://skynetcode.visualstudio.com/Libraries/_build/latest?definitionId=84&branchName=master)

Package to publish results of an ASP.NET Core health check to Datadog using Datadog's REST API. You can use this package in one of the two ways:

## In-process

This is where your ASP.NET Core application publishes its own health check results using  ASP.NET Core's default publishing service based on `IHostedService`. The way it will work is at regular intervals (a customisable number), the built in `HealthCheckPublisherHostedService` will collect health check results from all the health checks registered in your application using another built in `HealthCheckService` and then call into all the registered `IHealthCheckPublisher` implementations to publish the results of the health check.

The steps to register the Datadog API publisher are:

### Step 1: Add your custom health checks to IServiceCollection DI

```csharp
services.AddHealthChecks()
        .AddCheck<MyAppHealthCheck>("MyAppHealthCheck");        
```

### Step 2: Make sure you have the Datadog API key and application key and the following settings exist in your `secrets.json` (for local dev work) and your CI environment (you MUST have some secure way of providing these into your application)

```javascript
{
  "DatadogApiSettings": {
    "ApiKey": "<DATADOG API KEY>",
    "ApplicationKey": "<APPLICATION KEY THAT THE API KEY WILL WORK WITH>"
  }
}
```

### Step 3: Add this package (`AspNetCore.HealthCheckPublisher.DataDogApi`) to the `IHealthChecksBuilder` chain

```csharp
services
        .AddHealthChecks()
        .AddCheck<MyAppHealthCheck>("MyAppHealthCheck")
        .AddDatadogPublisher(
            Configuration.GetDatadogApiSettings(),
            new HealthReportOptionsBuilder(applicationPrefix: "myapi")
              .WithOptionalDefaultMetricTag("environment", Environment.EnvironmentName)
              .Build());
```

## Out-of-process

This is where the you ping the health endpoint on your ASP.NET Core application from an externally hosted service for e.g. a serverless function. This way if the application is unavailable the telemetry won't go down with it. Using this package out of process involves following steps:

### Step 1: Register an instance of the health reporter by a call to `AddDatadogHealthReporter` extension method available on the `ServiceCollection` passing in an instance of `DatadogApiSettings` that you can hydrate from `IConfiguration`:

```csharp
_services.AddDatadogHealthReporter(
  new DatadogApiSettings(
  configuration.GetValue<string>("DatadogApiSettingsApiKey"),
  configuration.GetValue<string>("DatadogApiSettingsApplicationKey"));                
```

### Step 2: Collect health check report from the target application using an `HttpClient`

### Step 3: Resolve the instance of `IApplicationHealthReporter` from the `ServiceCollection` either in a scope or via constructor injection

```csharp
var reporter = scope.ServiceProvider.GetService<IApplicationHealthReporter>();                
```

### Step 4: Send the health report via this reporter instance:

```csharp
await reporter.SendHealthReport(
  healthReport,
  new HealthReportOptionsBuilder(applicationPrefix: "myapi")
      .WithOptionalDefaultMetricTag(
          "Environment", configuration.GetValue("ASPNETCORE_ENVIRONMENT", "development"))
      .Build());
```