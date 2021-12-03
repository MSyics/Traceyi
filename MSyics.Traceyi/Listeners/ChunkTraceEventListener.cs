using System.Diagnostics;

namespace MSyics.Traceyi.Listeners;

public abstract class ChunkTraceEventListener : TraceEventListener
{
    readonly int chunk;
    readonly Dictionary<int, List<TraceEventArgs>> demuxes = new();

    public ChunkTraceEventListener(int demux = 1, int chunk = 1) : base(demux)
    {
        this.chunk = chunk;
        for (int i = 0; i < demux; i++)
        {
            demuxes[i] = new();
        }

        timer = CreateTimer();
    }

    #region Timer
    readonly Timer timer;
    int timerRunning;

    private Timer CreateTimer() => new(_ =>
    {
        try
        {
            StopTimer();

            foreach (var item in demuxes)
            {
                if (item.Value.Count > 0)
                {
                    lock (GlobalLock)
                    {
                        WriteCore(item.Value, item.Key);
                        item.Value.Clear();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    private bool TryStartTimer()
    {
        if (Interlocked.CompareExchange(ref timerRunning, 1, 0) is 0)
        {
            timer.Change(ChunkTimeout, Timeout.InfiniteTimeSpan);
            return true;
        }
        return false;
    }

    private bool TryStopTimer()
    {
        if (Interlocked.CompareExchange(ref timerRunning, 0, 1) is 1)
        {
            timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return true;
        }
        return false;
    }

    private void StopTimer() => Interlocked.Exchange(ref timerRunning, 0);
    #endregion

    /// <summary>
    /// 収集タイムアウト時間を取得または設定します。
    /// </summary>
    public TimeSpan ChunkTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);

    protected internal abstract void WriteCore(IEnumerable<TraceEventArgs> items, int index);

    protected internal override void WriteCore(TraceEventArgs e, int index)
    {
        var items = demuxes[index];
        lock (GlobalLock)
        {
            items.Add(e);
            TryStartTimer();

            if (items.Count >= chunk)
            {
                WriteCore(items, index);
                items.Clear();
                TryStopTimer();
            }
        }
    }

    protected override void DisposeManagedResources()
    {
        timer.Dispose();
        foreach (var item in demuxes)
        {
            if (item.Value.Count > 0)
            {
                WriteCore(item.Value, item.Key);
            }
        }
        demuxes.Clear();

        base.DisposeManagedResources();
    }
}
