using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByManual : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(SetupByManual);

        public void Setup()
        {
            Traceable.Add(
                name: "SetupByManual",
                filters: TraceFilters.All,
                useMemberInfo: false,
                listeners: x => Console.WriteLine(x.Message));

            Tracer = Traceable.Get("SetupByManual");
        }

        public void Show()
        {
            Tracer.Information("SetupByManual");
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
