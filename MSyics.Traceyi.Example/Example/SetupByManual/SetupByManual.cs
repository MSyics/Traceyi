using MSyics.Traceyi.Listeners;
using System;
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
                new ActionTraceListener(e => Console.WriteLine($"{e.Action} {e.Message} {e.Elapsed}")),
                new ConsoleLogger());

            Tracer = Traceable.Get(Name);
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
}
