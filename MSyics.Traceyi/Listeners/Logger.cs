/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータを記録します。これは抽象クラスです。
    /// </summary>
    public abstract class Logger : IDisposable, ITraceListener
    {
        #region Static Members
        private static object LockObj { get; } = new object();
        static Logger() { }
        #endregion

        /// <summary>
        /// グローバルロックを使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = true;

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; protected internal set; }

        void ITraceListener.OnTracing(object sender, TraceEventArg e)
        {
            if (UseLock)
            {
                lock (LockObj)
                {
                    Write(e);
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
        public abstract void Write(TraceEventArg e);

        /// <summary>
        /// 使用しているリソースを閉じます。
        /// </summary>
        public virtual void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 使用するリソースをすべて破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (disposing)
                {
                    DisposeManagedResources();
                }

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
