using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース操作を表します。
    /// </summary>
    public sealed class TraceScope
    {
        internal readonly static TraceScope NullScope = new();
        
        public TraceScope(bool withEntry)
        {
            WithEntry = withEntry;
        }

        public TraceScope()
        {
        }

        internal bool WithEntry { get; }

        /// <summary>
        /// スコープ ID を取得します。
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 親スコープ ID を取得します。
        /// </summary>
        public string ParentId { get; internal set; }

        /// <summary>
        /// スコープ番号を取得します。
        /// </summary>
        public int Depth { get; internal set; }

        /// <summary>
        /// 操作の識別子を取得します。
        /// </summary>
        public object OperationId { get; internal set; }

        /// <summary>
        /// 操作の開始日時を取得します。
        /// </summary>
        public DateTimeOffset Started { get; internal set; }
    }
}
