using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースのための一連の拡張メソッドを提供します。
    /// </summary>
    public static class TracerExtensions
    {
        /// <summary>
        /// コードブロックをトレースに参加させます。
        /// </summary>
        /// <param name="tracer">トレースオブジェクト</param>
        /// <param name="operationId">操作 ID</param>
        /// <param name="startMessage">開始メッセージ</param>
        /// <param name="stopMessage">終了メッセージ</param>
        public static TraceScope Scope(this Tracer tracer, object operationId = null, object startMessage = null, object stopMessage = null)
        {
            var scope = new TraceScope();
            scope.Start(tracer, operationId, startMessage, stopMessage);
            return scope;
        }

        /// <summary>
        /// 指定したフィルターに動作が含まれているかどうかを判定します。
        /// </summary>
        /// <param name="filters">トレースフィルター</param>
        /// <param name="action">トレース動作</param>
        /// <returns>含まれている場合は true、それ以外の場合は false。</returns>
        internal static bool Contains(this TraceFilters filters, TraceAction action)
        {
            if (filters.HasFlag(TraceFilters.None))
            {
                return false;
            }

            TraceFilters filter;
            switch (action)
            {
                case TraceAction.Trace:
                    filter = TraceFilters.Trace;
                    break;
                case TraceAction.Debug:
                    filter = TraceFilters.Debug;
                    break;
                case TraceAction.Info:
                    filter = TraceFilters.Info;
                    break;
                case TraceAction.Warning:
                    filter = TraceFilters.Warning;
                    break;
                case TraceAction.Error:
                    filter = TraceFilters.Error;
                    break;
                case TraceAction.Critical:
                    filter = TraceFilters.Critical;
                    break;
                case TraceAction.Start:
                    filter = TraceFilters.Start;
                    break;
                case TraceAction.Stop:
                    filter = TraceFilters.Stop;
                    break;
                default:
                    return false;
            }
            return (filter & filters) == filter;
        }
    }
}
