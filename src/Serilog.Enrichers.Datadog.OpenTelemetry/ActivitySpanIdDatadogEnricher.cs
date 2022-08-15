using Serilog.Core;
using Serilog.Events;
using System;
using System.Diagnostics;

namespace Serilog.Enrichers.Datadog.OpenTelemetry
{
    internal class ActivitySpanIdDatadogEnricher : ILogEventEnricher
    {
        /// <summary>
        /// The property name added to enriched log events.
        /// </summary>
        private const string SpanIdPropertyName = "dd.span_id";

        /// <summary>
        /// The cached last created "dd.span_id" property with some thread id. It is likely to be reused frequently so avoiding heap allocations.
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
        
            var stringSpanId = Activity.Current.SpanId.ToString();
            var ddSpanId = Convert.ToUInt64(stringSpanId, 16);

            var last = _lastValue;
            if (last == null || (ulong)((ScalarValue)last.Value).Value != ddSpanId)
                // no need to synchronize threads on write - just some of them will win
                _lastValue = last = new LogEventProperty(SpanIdPropertyName, new ScalarValue(ddSpanId));

            logEvent.AddPropertyIfAbsent(last);
        }
    }
}