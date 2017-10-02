/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        public readonly static TraceOperation NullOperation = new TraceOperation();

        /// <summary>
        /// 操作の識別子を取得します。
        /// </summary>
        public object OperationId { get; internal set; } = "";

        /// <summary>
        /// 操作の開始日時を取得します。
        /// </summary>
        public DateTime StartedDate { get; internal set; } = DateTime.MinValue;

        internal Guid ScopeId { get; set; } = Guid.Empty;
        internal bool UseScope => !this.ScopeId.Equals(Guid.Empty);
    }
}
