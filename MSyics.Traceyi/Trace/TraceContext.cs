/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.Generic;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース基本情報を表します。
    /// </summary>
    public sealed class TraceContext
    {
        /// <summary>
        /// 現在の活動識別子を取得または設定します。
        /// </summary>
        public object ActivityId { get; set; }

        internal Stack<TraceOperation> OperationStack
        {
            get
            {
                if (_operationStack == null)
                {
                    _operationStack = new Stack<TraceOperation>();
                }
                return _operationStack;
            }
        }
        private Stack<TraceOperation> _operationStack;

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
