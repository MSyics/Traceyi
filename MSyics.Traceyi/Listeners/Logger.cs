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
        private static Lazy<object> GlobalLock = new Lazy<object>(() => new object(), true);
        private static Lazy<AsyncLock> AsyncLock = new Lazy<AsyncLock>(() => new AsyncLock(), true);
        #endregion

        private long WritingCount = 0;

        /// <summary>
        /// グローバルロックを使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = true;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
        /// </summary>
        public bool UseAsync { get; set; } = true;

        public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromMilliseconds(-1);

        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; protected internal set; }

        async void ITraceListener.OnTracing(object sender, TraceEventArg e)
        {
            if (UseAsync)
            {
                Interlocked.Increment(ref WritingCount);
                if (UseLock)
                {
                    using (await AsyncLock.Value.LockAsync())
                    {
                        if (TokenSource.Token.IsCancellationRequested) return;
                        await Task.Run(() => Write(e), TokenSource.Token);
                    }
                }
                else
                {
                    if (TokenSource.Token.IsCancellationRequested) return;
                    await Task.Run(() => Write(e), TokenSource.Token);
                }
                Interlocked.Decrement(ref WritingCount);
            }
            else
            {
                if (UseLock)
                {
                    lock (GlobalLock.Value) { Write(e); }
                }
                else
                {
                    Write(e);
                }
            }
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public abstract void Write(TraceEventArg e);

        /// <summary>
        /// 使用しているリソースを閉じます。
        /// </summary>
        public void Close()
        {
            Dispose(true);

            Task.Run(() =>
            {
                while (WritingCount != 0) { }
            }).Wait(CloseTimeout);
            TokenSource.Cancel(false);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
        public void Dispose() => Close();

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
