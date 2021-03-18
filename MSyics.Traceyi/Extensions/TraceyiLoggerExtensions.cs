namespace Microsoft.Extensions.Logging
{
    using MSyics.Traceyi;
    using System;

    public static partial class TraceyiLoggerExtensions
    {
        #region BeginScope
        /// <summary>
        /// 
        /// </summary>
        public static IDisposable BeginScope(this ILogger logger, object message, Action<dynamic> extensions = null, object operationId = null) =>
            logger.BeginScope(new TraceyiLoggerParameters { Message = message, Extensions = extensions, OperationId = operationId, });

        /// <summary>
        /// 
        /// </summary>
        public static IDisposable BeginScope(this ILogger logger, Action<dynamic> extensions = null, object operationId = null) =>
            BeginScope(logger, null, extensions, operationId);
        #endregion

        #region Log
        /// <summary>
        /// 
        /// </summary>
        public static void Log(this ILogger logger, LogLevel logLevel, EventId eventId, Exception exception, object message, Action<dynamic> extensions) =>
            logger.Log(logLevel, eventId, new TraceyiLoggerParameters { Message = message, Extensions = extensions, }, exception, null);
        #endregion

        #region LogTrace
        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Trace, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Trace, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Trace, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Trace, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Trace, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Trace, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Trace, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogTrace(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Trace, default, null, null, extensions);
        #endregion

        #region LogDebug
        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Debug, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Debug, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Debug, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Debug, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Debug, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Debug, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Debug, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogDebug(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Debug, default, null, null, extensions);
        #endregion

        #region LogInformation
        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Information, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Information, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Information, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Information, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Information, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Information, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Information, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogInformation(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Information, default, null, null, extensions);
        #endregion

        #region LogWarning
        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Warning, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Warning, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Warning, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Warning, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Warning, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Warning, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Warning, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogWarning(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Warning, default, null, null, extensions);
        #endregion

        #region LogError
        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Error, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Error, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Error, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Error, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Error, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Error, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Error, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogError(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Error, default, null, null, extensions);
        #endregion

        #region LogCritical
        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Critical, eventId, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, EventId eventId, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Critical, eventId, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Critical, eventId, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, EventId eventId, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Critical, eventId, null, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, Exception exception, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Critical, default, exception, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, Exception exception, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Critical, default, exception, null, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, object message, Action<dynamic> extensions = null) =>
            Log(logger, LogLevel.Critical, default, null, message, extensions);

        /// <summary>
        /// 
        /// </summary>
        public static void LogCritical(this ILogger logger, Action<dynamic> extensions) =>
            Log(logger, LogLevel.Critical, default, null, null, extensions);
        #endregion
    }
}
