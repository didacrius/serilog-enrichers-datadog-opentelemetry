using System;
using Serilog.Configuration;

namespace Serilog.Enrichers.Datadog.OpenTelemetry
{
    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/> to add enrichers for <see cref="Activity.Current"/>
    /// capabilities.
    /// </summary>
    public static class ActivityContextDatadogLoggerConfigurationExtensions
    {
        /// <summary>
        /// Enrich log events with a dd.trace_id property containing the <see cref="Activity.Current.TraceId"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enrichmentConfiguration"/> is null.</exception>
        public static LoggerConfiguration WithDatadogTraceId(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<ActivityTraceIdDatadogEnricher>();
        }

        /// <summary>
        /// Enrich log events with a dd.span_id property containing the <see cref="Activity.Current.SpanId"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enrichmentConfiguration"/> is null.</exception>
        public static LoggerConfiguration WithDatadogSpanId(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<ActivitySpanIdDatadogEnricher>();
        }
    }
}