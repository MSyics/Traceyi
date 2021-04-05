using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースイベントを受信します。これは抽象クラスです。
    /// </summary>
    public abstract class TraceListener : IDisposable, ITraceListener
    {
        #region Static Members
        protected static readonly object GlobalLock = new();
        #endregion

        private readonly CancellationTokenSource cts = new();
        private readonly TraceEventChannel channel;

        public TraceListener(int concurrency = 1)
        {
            channel = new(Write, concurrency);
        }

        /// <summary>
        /// ロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = false;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
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
            if (cts.IsCancellationRequested) return;
            if (UseAsync)
            {
                try
                {
                    _ = WriteAsync(e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Write(e);
            }
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        protected internal abstract void WriteCore(TraceEventArgs e);

        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        private void Write(TraceEventArgs e)
        {
            if (UseLock)
            {
                lock (GlobalLock)
                {
                    try
                    {
                        WriteCore(e);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                try
                {
                    WriteCore(e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        private ValueTask WriteAsync(TraceEventArgs e) => channel.WriteAsync(e);

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
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
            channel.CloseAsync(CloseTimeout).Wait();
            cts.Dispose();

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
}
