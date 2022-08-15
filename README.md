# Serilog.Enrichers.Datadog.OpenTelemetry

![CI](https://github.com/didacrius/serilog-enrichers-datadog-opentelemetry/actions/workflows/ci.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/vpre/Serilog.Enrichers.Datadog.OpenTelemetry.svg)](https://www.nuget.org/packages/Serilog.Enrichers.Datadog.OpenTelemetry)

Enriches Serilog events with Datadog trace and span ids by taken the value of them from System.Diagnostics.Activity trace and span ids for Datadog traces correlation generated from OpenTelemetry.

To use the enricher, first install the NuGet package:

```powershell
Install-Package Serilog.Enrichers.Datadog.OpenTelemetry
```

Then, apply the enricher to your `LoggerConfiguration`:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithDatadogTraceId()
    .Enrich.WithDatadogSpanId()
    // ...other configuration...
    .CreateLogger();
```

The `WithDatadogTraceId()` enricher will add a `dd.trace_id` property with the current activity trace id as value to produced events.

The `WithDatadogSpanId()` enricher will add a `dd.span_id` property with the current activity span id as value to produced events.

### Included enrichers

The package includes:

 * `WithDatadogTraceId()` - adds a Datadog trace id to correlate logs with OpenTelemetry Datadog traces.
 * `WithDatadogSpanId()` - adds a Datadog span id to correlate logs with OpenTelemetry Datadog spans traces.

## Using it into an ASP.NET Core Web Application

This is a quite simple example what your `Program` file, in .NET 6, should contain in order for this enricher to work as expected:

```cs
var datadogConf = new DatadogConfiguration(url: builder.Configuration.GetValue<string>("Datadog:Endpoints:Logs"));

builder.Logging.ClearProviders()
    .AddSerilog(new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithDatadogTraceId()
        .Enrich.WithDatadogSpanId()
        .WriteTo.DatadogLogs(builder.Configuration.GetValue<string>("Datadog:ApiKey"),
            service: assemblyName.Name,
            configuration: datadogConf)
        .CreateLogger());

builder.Services
    .AddOpenTelemetryTracing(options =>
    {
        options.AddAspNetCoreInstrumentation()
            .AddOtlpExporter();
    });
```