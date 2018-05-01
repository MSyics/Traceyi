/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Listeners;
using System;

namespace MSyics.Traceyi
{
    internal class ActionTraceListener : ITraceListener
    {
        public Action<TraceEventArg> Listener { get; set; } = _ => { };

        public ActionTraceListener(Action<TraceEventArg> listener)
        {
            if (listener == null) return;
            Listener = listener;
        }

        public void OnTracing(object sender, TraceEventArg e) => Listener(e);

        public void Dispose()
        {
            Listener = null;
        }
    }
}
