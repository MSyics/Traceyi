using System;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// コードブロックをトレースに参加します。
    /// </summary>
    public sealed class TraceScope : IDisposable
    {
        private bool stopped = false;
        private Action<object> stop;

        /// <summary>
        /// 識別子を取得または設定します。
        /// </summary>
        public string Id { get; } = $"{DateTimeOffset.Now.Ticks}/{Thread.CurrentThread.ManagedThreadId}";

        internal void Start(Tracer tracer, object operationId = null, object startMessage = null, object stopMessage = null)
        {
            tracer.Start(operationId, startMessage, Id);
            stop = x => tracer.Stop(x ?? stopMessage, Id);
        }

        /// <summary>
        /// トレースのコードブロックから脱退します。
        /// </summary>
        /// <param name="stopMessage">メッセージ</param>
        public void Stop(object stopMessage = null)
        {
            if (stopped) { return; }
            stopped = true;
            stop?.Invoke(stopMessage);
            stop = null;
        }

        #region IDisposable Members
        /// <summary>
        /// トレースのコードブロックから脱退します。
        /// </summary>
        void IDisposable.Dispose() => Stop();
        #endregion
    }
}
