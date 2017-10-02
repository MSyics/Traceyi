using System;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi
{
    public interface ITraceListener
    {
        void OnTrace(object sender, TraceEventArg e);
    }
}
