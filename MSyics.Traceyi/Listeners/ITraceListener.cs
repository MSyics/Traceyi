/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        void OnTracing(object sender, TraceEventArg e);
    }
}
