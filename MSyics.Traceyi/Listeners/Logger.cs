/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
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
        private static object GlobalLock = new object();
        private static Lazy<AsyncLock> AsyncLock = new Lazy<AsyncLock>(() => new AsyncLock(), true);
        #endregion

        private CancellationTokenSource CTS = new CancellationTokenSource();
        private long AsyncWriteCount = 0;

        /// <summary>
        /// グローバルロックを使用するかどうか示す値を取得または設定します。
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
        public async void OnTracing(object sender, TraceEventArg e)
        {
            if (UseAsync)
            {
                if (CTS.IsCancellationRequested) return;
                try
                {
                    await WriteAsync(e);
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
        public async Task WriteAsync(TraceEventArg e)
        {
            if (CTS.IsCancellationRequested) return;
            try
            {
                Interlocked.Increment(ref AsyncWriteCount);

                if (UseLock)
                {
                    using (await AsyncLock.Value.LockAsync())
                    {
                        if (CTS.IsCancellationRequested) return;
                        await Task.Run(() => WriteCore(e), CTS.Token);
                    }
                }
                else
                {
                    if (CTS.IsCancellationRequested) return;
                    await Task.Run(() => WriteCore(e), CTS.Token);
                }

            }
            finally
            {
                Interlocked.Decrement(ref AsyncWriteCount);
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
        /// トレースデータを書き込みます。
        /// </summary>
        public abstract void WriteCore(TraceEventArg e);

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Task.Run(() => { while (AsyncWriteCount != 0) { } }).Wait(CloseTimeout);
            CTS.Cancel(false);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
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
