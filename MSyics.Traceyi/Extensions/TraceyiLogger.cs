namespace Microsoft.Extensions.Logging
{
    using MSyics.Traceyi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// ILogger の Traceyi 実装クラスです。
    /// </summary>
    internal class TraceyiLogger : ILogger
    {
        private static readonly string OriginalFormatKeyName = "{OriginalFormat}";
        private static readonly string PlaceholderKeyOperationId = "operationId".ToUpperInvariant();
        private readonly Tracer tracer;

        public TraceyiLogger(Tracer tracer)
        {
            this.tracer = tracer;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (!TryGetTraceAction(logLevel, out var traceAction)) { return true; }
            return tracer.Filters.Contains(traceAction);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return state switch
            {
                IEnumerable<KeyValuePair<string, object>> items => tracer.Scope(
                    items.FirstOrDefault(x => x.Key == OriginalFormatKeyName).Value, 
                    x => MakeExtensions(ref x, items)),
                TraceyiLoggerParameters p => tracer.Scope(
                    p.Message, 
                    p.Extensions, 
                    p.OperationId),
                _ => tracer.Scope(state),
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!TryGetTraceAction(logLevel, out var traceAction)) { return; }

            switch (state)
            {
                case IEnumerable<KeyValuePair<string, object>> items:
                    tracer.RaiseTracing(
                        traceAction,
                        items.FirstOrDefault(x => x.Key == OriginalFormatKeyName).Value,
                        x => MakeExtensions(ref x, items, eventId, exception));
                    break;
                case TraceyiLoggerParameters p:
                    tracer.RaiseTracing(
                        traceAction,
                        p.Message, x =>
                        {
                            MakeExtensions(ref x, eventId, exception);
                            p.Extensions?.Invoke(x);
                        });
                    break;
                default:
                    tracer.RaiseTracing(
                        traceAction,
                        state,
                        x => MakeExtensions(ref x, eventId, exception));
                    break;
            }
        }

        private bool TryGetTraceAction(LogLevel logLevel, out TraceAction traceAction)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    traceAction = TraceAction.Trace;
                    return true;
                case LogLevel.Debug:
                    traceAction = TraceAction.Debug;
                    return true;
                case LogLevel.Information:
                    traceAction = TraceAction.Info;
                    return true;
                case LogLevel.Warning:
                    traceAction = TraceAction.Warning;
                    return true;
                case LogLevel.Error:
                    traceAction = TraceAction.Error;
                    return true;
                case LogLevel.Critical:
                    traceAction = TraceAction.Critical;
                    return true;
                default:
                    traceAction = TraceAction.Trace;
                    return false;
            }
        }

#if NETCOREAPP
        private static bool GetKey(ReadOnlySpan<char> format, out string key)
        {
            var length = format.IndexOf("=>");
            if (length == -1)
            {
                length = format.Length;
            }

            int i = 0;
            foreach (var c in format[..length])
            {
                if (c == '{' || c == '}' || c == ':' || c == ',' || c == '|') break;
                ++i;
            }

            if (i == 0)
            {
                key = string.Empty;
                return false;
            }

            key = format[0..i].ToString();
            return true;
        }
#else
        private static bool GetKey(string format, out string key)
        {
            StringBuilder sb = new();
            var length = format.IndexOf("=>");
            if (length == -1)
            {
                length = format.Length;
            }

            foreach (var c in format.Take(length))
            {
                if (c == '{' || c == '}' || c == ':' || c == ',' || c == '|') break;
                sb.Append(c);
            }

            if (sb.Length == 0)
            {
                key = string.Empty;
                return false;
            }

            key = sb.ToString();
            return true;
        }
#endif

        private void MakeExtensions(ref dynamic x, IEnumerable<KeyValuePair<string, object>> items, EventId eventId = default, Exception exception = null)
        {
            MakeExtensions(ref x, eventId, exception);

            var extensions = (IDictionary<string, object>)x;
            foreach (var item in items.Where(x => x.Key != OriginalFormatKeyName))
            {
#if NETCOREAPP
                if (GetKey(item.Key.AsSpan(), out var key))
                {
                    extensions[key] = item.Value;
                }
#else
                if (GetKey(item.Key, out var key))
                {
                    extensions[key] = item.Value;
                }
#endif
                else
                {
                    extensions[item.Key] = item.Value;
                }
            }
        }

        private void MakeExtensions(ref dynamic x, EventId eventId = default, Exception exception = null)
        {
            if (eventId != default)
            {
                x.eventId = eventId.Id;

                if (eventId.Name is not null)
                {
                    x.eventName = eventId.Name;
                }
            }

            if (exception != null)
            {
                x.exception = exception;
            }
        }
    }
}