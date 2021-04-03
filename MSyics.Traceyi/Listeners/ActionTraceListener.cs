using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionTraceListener : ITraceListener
    {
        private Action<TraceEventArgs> Listener { get; set; } = _ => { };

        public ActionTraceListener(Action<TraceEventArgs> listener) => Listener = listener;

        public void OnTracing(object sender, TraceEventArgs e) => Listener?.Invoke(e);

        public void Dispose() => Listener = null;
    }
}
