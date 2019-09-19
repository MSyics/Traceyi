using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータを記録します。これは抽象クラスです。
    /// </summary>
    public abstract class Logger : IDisposable, ITraceListener
    {
        #region Static Members
        private static readonly object GlobalLock = new object();
        private static readonly Lazy<AsyncLock> AsyncLock = new Lazy<AsyncLock>(() => new AsyncLock(), true);
        #endregion

        private readonly CancellationTokenSource Cancellation = new CancellationTokenSource();
        private readonly ConcurrentQueue<TraceEventArg> TraceEventQueue = new ConcurrentQueue<TraceEventArg>();
        private long Dequeuing = 0;

        /// <summary>
        /// ロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = true;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
        /// </summary>
        public bool UseAsync { get; set; } = true;

        /// <summary>
        /// 終了を待機する時間間隔を取得または設定します。
        /// </summary>
        public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; protected internal set; }

        /// <summary>
        /// トレースイベントを処理します。
        /// </summary>
        public void OnTracing(object sender, TraceEventArg e)
        {
            if (UseAsync)
            {
                if (Cancellation.IsCancellationRequested) return;
                try
                {
                    _ = WriteAsync(e);
                }
                catch (TaskCanceledException)
                {
                }
            }
            else
            {
                Write(e);
            }
        }

        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        public void Write(TraceEventArg e)
        {
            if (UseLock)
            {
                lock (GlobalLock) { WriteCore(e); }
            }
            else
            {
                WriteCore(e);
            }
        }

        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        public async Task WriteAsync(TraceEventArg e)
        {
            if (Cancellation.IsCancellationRequested) return;

            TraceEventQueue.Enqueue(e);

            if (Interlocked.CompareExchange(ref Dequeuing, 1, 0) != 0) return;
            try
            {
                await Task.Run(() =>
                {
                    while (TraceEventQueue.TryDequeue(out var traceEvent) && !Cancellation.IsCancellationRequested)
                    {
                        Write(traceEvent);
                    }
                });
            }
            finally
            {
                Interlocked.Exchange(ref Dequeuing, 0);
            }
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        protected internal abstract void WriteCore(TraceEventArg e);

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            //Task.Run(() => { while (AsyncWriteCount != 0) { } }).Wait(CloseTimeout);

            Task.Run(() => { while (TraceEventQueue.Count != 0) ; }).Wait(CloseTimeout);
            Cancellation.Cancel(false);
            Cancellation.Dispose();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;
                if (disposing) { DisposeManagedResources(); }
                DisposeUnmanagedResources();
            }
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
