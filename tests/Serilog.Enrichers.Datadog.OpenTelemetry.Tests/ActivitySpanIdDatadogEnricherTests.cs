using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog.Sinks.InMemory;
using Xunit;

namespace Serilog.Enrichers.Datadog.OpenTelemetry.Tests;

public class ActivitySpanIdDatadogEnricherTests
{
    private const string SpanIdPropertyName = "dd.span_id";
    private static readonly ActivitySource Source = new (nameof(ActivitySpanIdDatadogEnricherTests));

    public ActivitySpanIdDatadogEnricherTests()
    {
        Sdk.CreateTracerProviderBuilder()
            .AddSource(nameof(ActivitySpanIdDatadogEnricherTests))
            .Build();
    }
    
    [Fact]
    public void OutputLogShouldContainDatadogSpanIdPropertyWhenActivityIsNotNull()
    {
        // Arrange
        var logger = new LoggerConfiguration()
            .Enrich.WithDatadogSpanId()
            .WriteTo.InMemory()
            .CreateLogger();

        const string message = "Dummy content";
        
        using var activity = Source.StartActivity(ActivityKind.Producer);
        
        // Act
        logger.Information(message);

        // Assert
        InMemorySink.Instance
            .LogEvents.First().Properties.Keys.Contains(SpanIdPropertyName)
            .Should()
            .BeTrue();
        
        InMemorySink.Instance
            .LogEvents.First().Properties[SpanIdPropertyName].ToString()
            .Should()
            .Be(Convert.ToUInt64(activity!.Context.SpanId.ToString(), 16).ToString());
    }
}