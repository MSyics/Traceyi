using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ILogger の Traceyi 実装クラスです。
    /// </summary>
    class TraceyiLogger : ILogger
    {
        private static readonly string PlaceholderKeyOperationId = "operationId".ToUpperInvariant();
        internal Tracer Tracer { get; private set; }

        public TraceyiLogger(Tracer tracer)
        {
            Tracer = tracer;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var message = TraceyiLoggerMessage.Create(state);
            if (message.HasPlaceholders)
            {
                var operaionId = message.Placeholders.FirstOrDefault(x => x.Key.ToUpperInvariant() == PlaceholderKeyOperationId);
                return Tracer.Scope(operationId: operaionId.Value, startMessage: message.ToString());
            }
            else
            {
                return Tracer.Scope(startMessage: state);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = TraceyiLoggerMessage.Create(eventId, state, exception, formatter);
            switch (logLevel)
            {
                case LogLevel.Trace:
                    Tracer.Trace(message);
                    break;
                case LogLevel.Debug:
                    Tracer.Debug(message);
                    break;
                case LogLevel.Information:
                    Tracer.Information(message);
                    break;
                case LogLevel.Warning:
                    Tracer.Warning(message);
                    break;
                case LogLevel.Error:
                    Tracer.Error(message);
                    break;
                case LogLevel.Critical:
                    Tracer.Critical(message);
                    break;
                case LogLevel.None:
                default:
                    break;
            }
        }
    }
}