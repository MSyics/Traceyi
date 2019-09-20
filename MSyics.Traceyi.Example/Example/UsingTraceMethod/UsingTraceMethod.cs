using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingTraceMethod : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(UsingTraceMethod);

        public void Setup()
        {
            Traceable.Add(@"Example\UsingTraceMethod\traceyi.json");
            Tracer = Traceable.Get();
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }

        public void Show()
        {
            using (Tracer.Scope())
            {
                Tracer.Start(Name);

                Tracer.Trace(Name);
                Tracer.Debug(Name);
                Tracer.Information(Name);
                Tracer.Warning(Name);
                Tracer.Error(Name);
                Tracer.Critical(Name);

                Tracer.Stop(Name);
            }
        }
    }
}
