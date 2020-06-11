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
        private readonly AsyncLocal<AsyncLocalStackNode<TraceOperation>> operationStackNode = new AsyncLocal<AsyncLocalStackNode<TraceOperation>>();
        private AsyncLocal<object> activityId = new AsyncLocal<object>();

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



    public class CorrelationManager
    {
        private readonly AsyncLocal<Guid> _activityId = new AsyncLocal<Guid>();
        private readonly AsyncLocal<StackNode> _stack = new AsyncLocal<StackNode>();
        private readonly Stack _stackWrapper;

        internal CorrelationManager()
        {
            _stackWrapper = new AsyncLocalStackWrapper(_stack);
        }

        public Stack LogicalOperationStack => _stackWrapper;

        //public void StartLogicalOperation() => StartLogicalOperation(Guid.NewGuid());

        public void StopLogicalOperation() => _stackWrapper.Pop();

        public Guid ActivityId { get { return _activityId.Value; } set { _activityId.Value = value; } }

        public void StartLogicalOperation(TraceOperation operationId)
        {
            if (operationId == null)
            {
                throw new ArgumentNullException(nameof(operationId));
            }

            _stackWrapper.Push(operationId);
        }

        private sealed class StackNode
        {
            internal StackNode(TraceOperation value, StackNode prev = null)
            {
                Value = value;
                Prev = prev;
                Count = prev != null ? prev.Count + 1 : 1;
            }

            internal int Count { get; }
            internal TraceOperation Value { get; }
            internal StackNode Prev { get; }
        }



        private sealed class AsyncLocalStackWrapper : Stack
        {
            private readonly AsyncLocal<StackNode> _stack;

            internal AsyncLocalStackWrapper(AsyncLocal<StackNode> stack)
            {
                Debug.Assert(stack != null);
                _stack = stack;
            }

            public override void Clear() => _stack.Value = null;

            public override object Clone() => new AsyncLocalStackWrapper(_stack);

            public override int Count => _stack.Value?.Count ?? 0;

            public override IEnumerator GetEnumerator() => GetEnumerator(_stack.Value);

            public override object Peek() => _stack.Value?.Value;

            public override bool Contains(object obj)
            {
                for (StackNode n = _stack.Value; n != null; n = n.Prev)
                {
                    if (obj == null)
                    {
                        if (n.Value == null) return true;
                    }
                    else if (obj.Equals(n.Value))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override void CopyTo(Array array, int index)
            {
                for (StackNode n = _stack.Value; n != null; n = n.Prev)
                {
                    array.SetValue(n.Value, index++);
                }
            }

            private IEnumerator GetEnumerator(StackNode n)
            {
                while (n != null)
                {
                    yield return n.Value;
                    n = n.Prev;
                }
            }

            public override object Pop()
            {
                StackNode n = _stack.Value;
                if (n == null)
                {
                    base.Pop(); // used to throw proper exception
                }
                _stack.Value = n.Prev;
                return n.Value;
            }

            public override void Push(object obj)
            {
                _stack.Value = new StackNode(obj as TraceOperation, _stack.Value);
            }

            public override object[] ToArray()
            {
                StackNode n = _stack.Value;
                if (n == null)
                {
                    return Array.Empty<object>();
                }

                var results = new List<object>();
                do
                {
                    results.Add(n.Value);
                    n = n.Prev;
                }
                while (n != null);
                return results.ToArray();
            }
        }
    }
}
