using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ILogger 実装 TraceyiLogger のパラメーターを表します。
    /// </summary>
    internal sealed class TraceyiLoggerParameters
    {
        public object Message { get; set; }
        public Action<dynamic> Extensions { get; set; }
        public object ScopeLabel { get; set; }
    }
}