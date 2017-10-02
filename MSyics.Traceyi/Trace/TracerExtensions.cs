using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        /// <param name="filter">トレースフィルター</param>
        /// <param name="action">トレース動作</param>
        /// <returns>含まれている場合は true、それ以外の場合は false。</returns>
        internal static bool Contains(this TraceFilters filter, TraceAction action)
        {
            try
            {
                var actions = (TraceFilters)Enum.Parse(typeof(TraceFilters), action.ToString());
                return (actions & filter) == actions;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// トレースの設定を行います。
        /// </summary>
        public static Tracer Configure(this Tracer tracer, Action<TracerConfiguration> settings)
        {
            settings(new TracerConfiguration(tracer));
            return tracer;
        }
    }
}
