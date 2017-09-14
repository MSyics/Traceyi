using System;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータを記録します。これは抽象クラスです。
    /// </summary>
    public abstract class Log : IDisposable
    {
        #region Static Members
        private static readonly object m_thisLock = new object();
        static Log() { }
        #endregion

        /// <summary>
        /// グローバルロックを使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseGlobalLock { get; set; } = false;

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; protected internal set; }

        internal void OnTrace(object sender, TraceEventArg e)
        {
            if (this.UseGlobalLock)
            {
                lock (Log.m_thisLock)
                {
                    Write(e.Message, e.DateTime, e.Action);
                }
            }
            else
            {
                Write(e.Message, e.DateTime, e.Action);
            }
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public abstract void Write(object message, DateTime dateTime, TraceAction action, TraceEventCacheData cacheData);

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public void Write(object message, DateTime dateTime, TraceAction action)
        {
            Write(message, dateTime, action, new TraceEventCacheData());
        }

        /// <summary>
        /// 使用しているリソースを閉じます。
        /// </summary>
        public virtual void Close()
        {
            Dispose();
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 使用するリソースをすべて破棄します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;

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
