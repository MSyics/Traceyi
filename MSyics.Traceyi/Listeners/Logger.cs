using System;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータを記録します。これは抽象クラスです。
    /// </summary>
    public abstract class Logger : TraceListener, IDisposable, ITraceListener
    {
        public Logger(int concurrency = 1) : base(concurrency)
        {
        }
    }
}