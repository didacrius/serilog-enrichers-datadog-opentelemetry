using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog.Sinks.InMemory;
using Xunit;

namespace Serilog.Enrichers.Datadog.OpenTelemetry.Tests;

public class ActivityTraceIdDatadogEnricherTests
{
    private const string TraceIdPropertyName = "dd.trace_id";
    private static readonly ActivitySource Source = new (nameof(ActivityTraceIdDatadogEnricherTests));

    public ActivityTraceIdDatadogEnricherTests()
    {
        Sdk.CreateTracerProviderBuilder()
            .AddSource(nameof(ActivityTraceIdDatadogEnricherTests))
            .Build();
    }
    
    [Fact]
    public void OutputLogShouldContainDatadogTraceIdPropertyWhenActivityIsNotNull()
    {
        // Arrange
        var logger = new LoggerConfiguration()
            .Enrich.WithDatadogTraceId()
            .WriteTo.InMemory()
            .CreateLogger();

        const string message = "Dummy content";
        
        using var activity = Source.StartActivity(ActivityKind.Producer);
        
        // Act
        logger.Information(message);

        // Assert
        InMemorySink.Instance
            .LogEvents.First().Properties.Keys.Contains(TraceIdPropertyName)
            .Should()
            .BeTrue();
        
        InMemorySink.Instance
            .LogEvents.First().Properties[TraceIdPropertyName].ToString()
            .Should()
            .Be(Convert.ToUInt64(activity!.Context.TraceId.ToString().Substring(16), 16).ToString());
    }
}