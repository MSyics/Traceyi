using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class SetupByManual : Example
    {
        public override string Name => nameof(SetupByManual);

        public override void Setup()
        {
            Traceable.Add(
                name: Name,
                filters: TraceFilters.All,
                listeners: x => Console.WriteLine(x));

            Tracer = Traceable.Get(Name);
        }

        public override Task ShowAsync()
        {
            using (Tracer.Scope())
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
}
