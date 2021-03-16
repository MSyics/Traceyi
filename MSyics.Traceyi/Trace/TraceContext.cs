using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース基本情報を表します。
    /// </summary>
    public sealed class TraceContext
    {
        readonly AsyncLocal<AsyncLocalStackNode<TraceOperation>> operationStackNode = new();
        readonly AsyncLocal<object> activityId = new();

        internal TraceContext()
        {
            OperationStack = new AsyncLocalStack<TraceOperation>(operationStackNode);
        }

        internal AsyncLocalStack<TraceOperation> OperationStack { get; private set; }

        /// <summary>
        /// 現在の活動識別子を取得または設定します。
        /// </summary>
        public object ActivityId { get => activityId.Value; set => activityId.Value = value; }

        /// <summary>
        /// 現在のトレース操作情報を取得します。
        /// </summary>
        public TraceOperation CurrentOperation => OperationStack.Count == 0 ? TraceOperation.NullOperation : OperationStack.Peek();

        /// <summary>
        /// トレース操作情報の一覧を取得します。
        /// </summary>
        public TraceOperation[] Operations => OperationStack.ToArray();

        /// <summary>
        /// トレース基本情報をリフレッシュします。
        /// </summary>
        public void Refresh()
        {
            ActivityId = null;
            OperationStack.Clear();
        }
    }
}
