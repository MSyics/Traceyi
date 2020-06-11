using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレース操作のノードを表します。
    /// </summary>
    internal sealed class AsyncLocalStackNode<T>
    {
        internal AsyncLocalStackNode(T element, AsyncLocalStackNode<T> prev = null)
        {
            Element = element;
            Prev = prev;
            Count = prev == null ? 1 : prev.Count + 1;
        }

        internal int Count { get; }

        internal T Element { get; }

        internal AsyncLocalStackNode<T> Prev { get; }
    }
}
