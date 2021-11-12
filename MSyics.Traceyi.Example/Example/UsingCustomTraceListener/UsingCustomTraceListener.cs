using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;
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
            using (Tracer.Scope(label: Name))
            {
                Tracer.Information(Name);
            }

            return Task.CompletedTask;
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }

    class CustomElement : TraceEventListenerElement
    {
        public int Value { get; set; }

        public override ITraceEventListener GetRuntimeObject()
        {
            return new CustomTraceListener
            {
                Value = Value,
            };
        }
    }

    class CustomTraceListener : ITraceEventListener
    {
        public int Value { get; set; }

        public void OnTracing(object sender, TraceEventArgs e)
        {
            Console.WriteLine($"{e.Action} {e.Message} {Value}");
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
