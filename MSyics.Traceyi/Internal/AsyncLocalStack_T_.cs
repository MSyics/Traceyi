﻿using System.Collections;

namespace MSyics.Traceyi;

/// <summary>
/// 非同期制御中のローカルなスタックを表します。
/// </summary>
internal sealed class AsyncLocalStack<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection
{
    private readonly AsyncLocal<AsyncLocalStackNode<T>> local;

    public AsyncLocalStack(AsyncLocal<AsyncLocalStackNode<T>> local)
    {
        this.local = local;
    }

    /// <summary>
    /// スタックの先頭にあるオブジェクトを削除して返します。
    /// </summary>
    public T Pop()
    {
        var node = local.Value;
        if (node is null)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        local.Value = node.Prev;
        return node.Element;
    }

    /// <summary>
    /// スタックの先頭にあるオブジェクトの削除を試みます。
    /// </summary>
    public bool TryPop(out T value)
    {
        var node = local.Value;
        if (node is null)
        {
            value = default;
            return false;
        }

        local.Value = node.Prev;
        value = node.Element;
        return true;
    }

    /// <summary>
    /// スタックの先頭にあるオブジェクトの削除を試みます。
    /// </summary>
    public bool TryPop()
    {
        var node = local.Value;
        if (node is null)
        {
            return false;
        }

        local.Value = node.Prev;
        return true;
    }

    /// <summary>
    /// スタックの先頭にオブジェクトを挿入します。
    /// </summary>
    public void Push(T element) => local.Value = new AsyncLocalStackNode<T>(element, local.Value);

    /// <summary>
    /// スタックの先頭にあるオブジェクトを削除しないで返します。
    /// </summary>
    /// <returns></returns>
    public T Peek() => local.Value is null ? default : local.Value.Element;

    /// <summary>
    /// スタックからすべてのオブジェクトを削除します。
    /// </summary>
    public void Clear() => local.Value = null;

    /// <summary>
    /// スタックが持つオブジェクト数を取得します。
    /// </summary>
    public int Count => local.Value is null ? 0 : local.Value.Count;

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
        for (var node = local.Value; node is not null; node = node.Prev)
        {
            yield return node.Element;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return ((IEnumerable<T>)this).GetEnumerator();
    }
}
