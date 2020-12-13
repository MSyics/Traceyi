﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ILoggingBuilder の Traceyi 拡張メソッド群を提供します。
    /// </summary>
    public static class TraceyiLoggingBuilderExtensions
    {
        /// <summary>
        /// 登録します。
        /// </summary>
        private static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, TraceyiLoggerProvider>(x => new TraceyiLoggerProvider()));
            return builder;
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, IConfiguration configuration, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(configuration, usable);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string jsonFile, Action<ITraceListenerElementConfiguration> usable = null)
        {
            Traceable.Add(jsonFile, usable);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string name = "", TraceFilters filters = TraceFilters.All, params Action<TraceEventArgs>[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return builder.AddTraceyi();
        }

        /// <summary>
        /// 登録します。
        /// </summary>
        public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string name = "", TraceFilters filters = TraceFilters.All, params ITraceListener[] listeners)
        {
            Traceable.Add(name, filters, listeners);
            return builder.AddTraceyi();
        }
    }
}