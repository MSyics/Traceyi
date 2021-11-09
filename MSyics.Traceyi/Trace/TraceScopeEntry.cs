namespace MSyics.Traceyi;

/// <summary>
/// コードブロックをトレースに参加します。
/// </summary>
public sealed class TraceScopeEntry : IDisposable
{
    private bool stopped = false;
    private Action<object, Action<dynamic>> stop;

    internal void Start(Tracer tracer, object message, Action<dynamic> extensions, object label)
    {
        var scopeId = tracer.Start(message, extensions, label, true);
        stop = (m, e) => tracer.Stop(scopeId, DateTimeOffset.Now, m, e);
    }

    /// <summary>
    /// トレースのコードブロックから脱退します。
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="extensions"></param>
    public void Stop(object message, Action<dynamic> extensions = null)
    {
        if (stopped) return;

        stopped = true;
        stop?.Invoke(message, extensions);
        stop = null;
    }

    /// <summary>
    /// トレースのコードブロックから脱退します。
    /// </summary>
    /// <param name="extensions"></param>
    public void Stop(Action<dynamic> extensions = null) => Stop(null, extensions);

    #region IDisposable Members
    /// <summary>
    /// トレースのコードブロックから脱退します。
    /// </summary>
    void IDisposable.Dispose() => Stop();
    #endregion
}
