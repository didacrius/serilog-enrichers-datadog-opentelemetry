using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace Serilog.Enrichers.Datadog.OpenTelemetry
{
    internal class ActivityTraceIdDatadogEnricher : ILogEventEnricher
    {
        /// <summary>
        /// The property name added to enriched log events.
        /// </summary>
        private const string TraceIdPropertyName = "dd.trace_id";

        /// <summary>
        /// The cached last created "dd.trace_id" property with some thread id. It is likely to be reused frequently so avoiding heap allocations.
        /// </summary>
        private LogEventProperty _lastValue;

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current is null)
                return;
        
            var stringTraceId = Activity.Current.TraceId.ToString();
            var ddTraceId = Convert.ToUInt64(stringTraceId.Substring(16), 16);

            var last = _lastValue;
            if (last == null || (ulong)((ScalarValue)last.Value).Value != ddTraceId)
                // no need to synchronize threads on write - just some of them will win
                _lastValue = last = new LogEventProperty(TraceIdPropertyName, new ScalarValue(ddTraceId));

            logEvent.AddPropertyIfAbsent(last);
        }
    }   
}