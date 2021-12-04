using System.Diagnostics;

namespace MSyics.Traceyi.Listeners;

/// <summary>
/// トレースイベントを受信します。これは抽象クラスです。
/// </summary>
public abstract class TraceEventListener : ITraceEventListener, IDisposable, IAsyncDisposable
{
    #region Static Members
    protected static readonly object GlobalLock = new();
    #endregion

    private readonly CancellationTokenSource cts = new();
    private readonly TraceEventChannel channel;

    public TraceEventListener(int demux = 1)
    {
        if (UseAsync)
        {
            channel = new(Write, demux);
            channel.Open();
        }
    }

    /// <summary>
    /// ロックを使用するかどうかを示す値を取得します。
    /// </summary>
    public bool UseLock { get; set; } = false;

    /// <summary>
    /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得します。
    /// </summary>
    public bool UseAsync { get; set; } = true;

    /// <summary>
    /// 終了を待機する時間間隔を取得または設定します。
    /// </summary>
    public TimeSpan CloseTimeout { get; set; } = Timeout.InfiniteTimeSpan;

    /// <summary>
    /// 名前を取得または設定します。
    /// </summary>
    public string Name { get; protected internal set; }

    /// <summary>
    /// トレースイベントを処理します。
    /// </summary>
    public void OnTracing(object sender, TraceEventArgs e)
    {
        if (cts.IsCancellationRequested) { return; }
        if (UseAsync)
        {
            _ = WriteAsync(e);
        }
        else
        {
            Write(e);
        }
    }

    /// <summary>
    /// トレースデータを書き込みます。
    /// </summary>
    protected internal abstract void WriteCore(TraceEventArgs e, int index);

    /// <summary>
    /// トレースイベント情報を書き込みます。
    /// </summary>
    private void Write(TraceEventArgs e, int index = 0)
    {
        if (UseLock)
        {
            lock (GlobalLock)
            {
                try
                {
                    WriteCore(e, index);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
        else
        {
            try
            {
                WriteCore(e, index);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    /// <summary>
    /// トレースイベント情報を書き込みます。
    /// </summary>
    private async ValueTask WriteAsync(TraceEventArgs e)
    {
        try
        {
            await channel.WriteAsync(e).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// リソースを破棄したかどうかを示す値を取得します。
    /// </summary>
    public bool Disposed { get; private set; } = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (Disposed) { return; }
        Disposed = true;

        cts.Cancel(false);
        channel.Close(CloseTimeout);

        if (disposing) { DisposeManagedResources(); }
        DisposeUnmanagedResources();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (Disposed) { return; }
        Disposed = true;

        cts.Cancel(false);
        await channel.CloseAsync(CloseTimeout);

        if (disposing) { DisposeManagedResources(); }
        DisposeUnmanagedResources();
    }

    /// <summary>
    /// マネージリソースを破棄します。
    /// </summary>
    protected virtual void DisposeManagedResources() { }

    /// <summary>
    /// アンマネージリソースを破棄します。
    /// </summary>
    protected virtual void DisposeUnmanagedResources() { }
}
