using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース操作のノードを表します。
    /// </summary>
    internal sealed class AsyncLocalStackNode<T>
    {
        public AsyncLocalStackNode(T element, AsyncLocalStackNode<T> prev = null)
        {
            Element = element;
            Prev = prev;
            Count = prev == null ? 1 : prev.Count + 1;
        }

        public int Count { get; }

        public T Element { get; }

        public AsyncLocalStackNode<T> Prev { get; }
    }
}
