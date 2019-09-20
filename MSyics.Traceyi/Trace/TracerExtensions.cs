using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ロギングのための一連の拡張メソッドを提供します。
    /// </summary>
    public static class TracerExtensions
    {
        /// <summary>
        /// コードブロックをトレースに参加させます。
        /// </summary>
        /// <param name="target">トレースオブジェクト</param>
        /// <param name="operationId">操作ID</param>
        public static TraceOperationScope Scope(this Tracer target, object operationId) => new TraceOperationScope(target, operationId);

        /// <summary>
        /// コードブロックをトレースに参加させます。
        /// </summary>
        /// <param name="target">トレースオブジェクト</param>
        public static TraceOperationScope Scope(this Tracer target) => new TraceOperationScope(target);

        /// <summary>
        /// 指定したフィルターに動作が含まれているかどうかを判定します。
        /// </summary>
        /// <param name="filters">トレースフィルター</param>
        /// <param name="action">トレース動作</param>
        /// <returns>含まれている場合は true、それ以外の場合は false。</returns>
        internal static bool Contains(this TraceFilters filters, TraceAction action)
        {
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
                case TraceAction.Elapsed:
                    filter = TraceFilters.Elapsed;
                    break;
                default:
                    filter = TraceFilters.None;
                    break;
            }
            return (filter & filters) == filter;
        }
    }
}
