using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingCustomTraceListener : ITrecyiExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            Traceable.Add("Traceyi.json", usable => usable.In<CustomElement>("Custom"));

            Tracer = Traceable.Get("UsingCustomTraceListener");
        }

        public void Test()
        {
            Tracer.Information("UsingCustomTraceListener");
        }
    }

    class CustomElement : TraceListenerElement
    {
        public override ITraceListener GetRuntimeObject()
        {
            return new CustomTraceListener();
        }
    }

    class CustomTraceListener : ITraceListener
    {
        public void OnTracing(object sender, TraceEventArg e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
