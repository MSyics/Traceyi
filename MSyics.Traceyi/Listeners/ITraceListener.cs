using System;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// Trace オブジェクトのトレースイベントを処理する機能を提供します。
    /// </summary>
    public interface ITraceListener : IDisposable
    {
        /// <summary>
        /// トレースイベントを処理します。
        /// </summary>
        void OnTracing(object sender, TraceEventArgs e);
    }
}
