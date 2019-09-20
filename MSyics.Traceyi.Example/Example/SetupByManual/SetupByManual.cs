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
                name: Name,
                filters: TraceFilters.All,
                useMemberInfo: true,
                listeners: x => Console.WriteLine(x));

            Tracer = Traceable.Get(Name);
        }

        public void Show()
        {
            using (Tracer.Scope())
            {
                Tracer.Information(Name);
            }
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
