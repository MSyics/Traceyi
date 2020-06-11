﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace MSyics.Traceyi
{
    internal sealed class AsyncLocalStack<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection
    {
        private readonly AsyncLocal<AsyncLocalStackNode<T>> local;

        public AsyncLocalStack(AsyncLocal<AsyncLocalStackNode<T>> local)
        {
            this.local = local;
        }

        public T Pop()
        {
            var node = local.Value;
            if (node == null)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            local.Value = node.Prev;
            return node.Element;
        }

        public void Push(T element) => local.Value = new AsyncLocalStackNode<T>(element, local.Value);

        public T Peek() => local.Value == null ? default : local.Value.Element;

        public void Clear() => local.Value = null;

        public int Count => local.Value == null ? 0 : local.Value.Count;

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        void ICollection.CopyTo(Array array, int index)
        {
            foreach (var element in this)
            {
                index += 1;
                array.SetValue(element, index);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (var node = local.Value; node != null; node = node.Prev)
            {
                yield return node.Element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}