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
    /// ILoggerFactory の Traceyi 拡張メソッドを提供します。
    /// </summary>
    public static class TraceyiLoggerFactoryExtensions
    {
        /// <summary>
        /// 登録します。
        /// </summary>
        private static ILoggerFactory AddTraceyi(this ILoggerFactory factory)
        {
            factory.AddProvider(new TraceyiLoggerProvider());
            return factory;
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, IConfiguration configuration, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(configuration, usable);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string jsonFile, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(jsonFile, usable);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string name = "", TraceFilters filters = TraceFilters.All, params Action<TraceEventArgs>[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return factory.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggerFactory AddTraceyi(this ILoggerFactory factory, string name = "", TraceFilters filters = TraceFilters.All, params ITraceListener[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return factory.AddTraceyi();
        }
    }
}