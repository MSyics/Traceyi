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
            this.Target = target;
            this.Target.Start(operationId, null, this.Id);
        }

        /// <summary>
        /// TraceOperationScope クラスのイスタンスを初期化します。
        /// </summary>
        public TraceOperationScope(Tracer target)
        {
            this.Target = target;
            this.Target.Start(null, null, this.Id);
        }
      
        #region IDisposable Members
        /// <summary>
        /// トレースのコードブロックから脱退します。
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                this.Target.Stop(null, this.Id);
                _disposed = true;
            }
        }
        private bool _disposed = false;
        #endregion // End IDisposable Members
    }
}
