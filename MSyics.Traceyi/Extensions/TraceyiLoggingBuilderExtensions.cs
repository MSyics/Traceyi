using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// 
    /// </summary>
    public static class TraceyiLoggingBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, TraceyiLoggerProvider>(x => new TraceyiLoggerProvider()));
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="usable"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, IConfiguration configuration, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(configuration, usable);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jsonFile"></param>
        /// <param name="usable"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string jsonFile, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(jsonFile, usable);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="filters"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string name = "", TraceFilters filters = TraceFilters.All, params Action<TraceEventArg>[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="filters"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string name = "", TraceFilters filters = TraceFilters.All, params ITraceListener[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return builder.AddTraceyi();
        }
    }
}