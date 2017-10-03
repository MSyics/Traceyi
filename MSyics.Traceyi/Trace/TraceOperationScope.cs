/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

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
        public Guid Id { get; } = Guid.NewGuid();

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
            if (!_disposed)
            {
                Target.Stop(null, Id);
                _disposed = true;
            }
        }
        private bool _disposed = false;
        #endregion // End IDisposable Members
    }
}
