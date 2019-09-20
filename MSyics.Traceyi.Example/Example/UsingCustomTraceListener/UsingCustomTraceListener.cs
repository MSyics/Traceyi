using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingCustomTraceListener : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(UsingCustomTraceListener);

        public void Setup()
        {
            Traceable.Add(@"example\UsingCustomTraceListener\traceyi.json", usable => usable.In<CustomElement>("Custom"));

            Tracer = Traceable.Get();
        }

        public void Show()
        {
            Tracer.Information(Name);
        }

        public void Teardown()
        {
            Traceable.Shutdown();
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
        public void Dispose()
        {
        }

        public void OnTracing(object sender, TraceEventArg e)
        {
            Console.WriteLine(e.Message);
        }

        public void OnTracing(TraceEventArg e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
