using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    internal class ActionTraceListener : ITraceListener
    {
        public Action<TraceEventArg> Listener { get; set; } = _ => { };

        public ActionTraceListener(Action<TraceEventArg> listener) => Listener = listener;

        public void OnTracing(object sender, TraceEventArg e) => Listener?.Invoke(e);

        public void Dispose() => Listener = null;
    }
}
