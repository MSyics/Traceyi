using System;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// コードブロックをトレースに参加します。
    /// </summary>
    public sealed class TraceScope : IDisposable
    {
        private Action action;

        /// <summary>
        /// 識別子を取得または設定します。
        /// </summary>
        public string Id { get; } = $"{DateTimeOffset.Now.Ticks}/{Thread.CurrentThread.ManagedThreadId}";

        /// <summary>
        /// TraceScope クラスのイスタンスを初期化します。
        /// </summary>
        public TraceScope(Tracer tracer, object operationId = null, object startMessage = null, object stopMessage = null)
        {
            tracer.Start(operationId, startMessage, Id);
            action = () => tracer.Stop(stopMessage, Id);
        }

        #region IDisposable Members
        private bool disposed = false;

        /// <summary>
        /// トレースのコードブロックから脱退します。
        /// </summary>
        public void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            action?.Invoke();
            action = null;
        }
        #endregion
    }
}
