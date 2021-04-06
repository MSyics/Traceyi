using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    internal class ActionTraceEventListener : ITraceEventListener
    {
        private Action<TraceEventArgs> Listener { get; set; } = _ => { };

        public ActionTraceEventListener(Action<TraceEventArgs> listener) => Listener = listener;

        public void OnTracing(object sender, TraceEventArgs e) => Listener?.Invoke(e);

        public void Dispose() => Listener = null;
    }
}
