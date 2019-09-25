using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingCustomTraceListener : Example
    {
        public override string Name => nameof(UsingCustomTraceListener);

        public override void Setup()
        {
            Traceable.Add(@"example\UsingCustomTraceListener\traceyi.json", usable => usable.In<CustomElement>("Custom"));

            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            Tracer.Information(Name);

            return Task.CompletedTask;
        }

        public override void Teardown()
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
