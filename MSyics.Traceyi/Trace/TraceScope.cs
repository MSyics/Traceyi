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

        internal void Start(Tracer tracer, object operationId = null, object startMessage = null, object stopMessage = null)
        {
            var scopeId = tracer.StartCore(operationId, startMessage);
            stop = x => tracer.Stop(x ?? stopMessage, scopeId);
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
