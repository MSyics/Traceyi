using System;
using System.Threading;

namespace MSyics.Traceyi
{
    /// <summary>
    /// コードブロックをトレースに参加します。
    /// </summary>
    public sealed class TraceOperationScope : IDisposable
    {
        private Tracer Target { get; set; }

        /// <summary>
        /// 識別子を取得または設定します。
        /// </summary>
        public string Id { get; } = $"{DateTimeOffset.Now.Ticks}.{Thread.CurrentThread.ManagedThreadId}";

        /// <summary>
        /// TraceOperationScope クラスのイスタンスを初期化します。
        /// </summary>
        public TraceOperationScope(Tracer target, object operationId)
        {
            Target = target;
            Target.Start(operationId, null, Id);
        }

        /// <summary>
        /// TraceOperationScope クラスのイスタンスを初期化します。
        /// </summary>
        public TraceOperationScope(Tracer target)
        {
            Target = target;
            Target.Start(null, null, Id);
        }

        #region IDisposable Members
        /// <summary>
        /// トレースのコードブロックから脱退します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) { return; }
            Target.Stop(null, Id);
            _disposed = true;
        }
        private bool _disposed = false;
        #endregion // End IDisposable Members
    }
}
