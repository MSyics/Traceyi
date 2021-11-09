using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSyics.Traceyi;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// ILoggingBuilder の Traceyi 拡張メソッド群を提供します。
/// </summary>
public static partial class TraceyiLoggingBuilderExtensions
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
    public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, IConfiguration configuration, Action<ITraceEventListenerElementConfiguration> usable = null)
    {
        Traceable.Add(configuration, usable);
        return builder.AddTraceyi();
    }

    /// <summary>
    /// 登録します。
    /// </summary>
    public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string jsonFile, Action<ITraceEventListenerElementConfiguration> usable = null)
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
    public static ILoggingBuilder AddTraceyi(this ILoggingBuilder builder, string name = "", TraceFilters filters = TraceFilters.All, params ITraceEventListener[] listeners)
    {
        Traceable.Add(name, filters, listeners);
        return builder.AddTraceyi();
    }
}
