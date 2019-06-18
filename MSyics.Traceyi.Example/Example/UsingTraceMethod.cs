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
            Traceable.Add(@"config\settings.json");
            Tracer = Traceable.Get();
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }

        public void Show()
        {
            Tracer.Information("UsingTraceMethod");
            Tracer.Debug("UsingTraceMethod");
            Tracer.Warning("UsingTraceMethod");
            Tracer.Error("UsingTraceMethod");
            Tracer.Start("UsingTraceMethod");
            Tracer.Stop("UsingTraceMethod");
        }
    }
}
