using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSyics.Traceyi;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSyics.Traceyi
{
    /// <summary>
    /// 
    /// </summary>
    public static class TraceyiLoggerFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        private static ILoggerFactory AddTraceyi(this ILoggerFactory factory)
        {
            factory.AddProvider(new TraceyiLoggerProvider());
            return factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="configuration"></param>
        /// <param name="usable"></param>
        /// <returns></returns>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, IConfiguration configuration, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(configuration, usable);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="jsonFile"></param>
        /// <param name="usable"></param>
        /// <returns></returns>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string jsonFile, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(jsonFile, usable);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <param name="filters"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string name = "", TraceFilters filters = TraceFilters.All, params Action<TraceEventArg>[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <param name="filters"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string name = "", TraceFilters filters = TraceFilters.All, params ITraceListener[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return factory.AddTraceyi();
        }
    }
}