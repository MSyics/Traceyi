using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース操作を表します。
    /// </summary>
    public sealed class TraceOperation
    {
        /// <summary>
        /// 操作が無い状態を表します。
        /// </summary>
        internal readonly static TraceOperation NullOperation = new TraceOperation();

        /// <summary>
        /// スコープ ID を取得します。
        /// </summary>
        public string ScopeId { get; internal set; }

        /// <summary>
        /// スコープを使用しているかどうかを示す値を取得します。
        /// </summary>
        public bool UseScope => ScopeId != null;

        /// <summary>
        /// 親スコープ ID を取得します。
        /// </summary>
        public string ParentId { get; internal set; }

        /// <summary>
        /// スコープ番号を取得します。
        /// </summary>
        public int ScopeNumber { get; internal set; }

        /// <summary>
        /// 操作の識別子を取得します。
        /// </summary>
        public object Id { get; internal set; } = "";

        /// <summary>
        /// 操作の開始日時を取得します。
        /// </summary>
        public DateTime Started { get; internal set; } = DateTime.MinValue;

        public DateTime Created { get; } = DateTime.Now;
    }
}
