using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingLayout : Example
    {
        public override string Name => nameof(UsingLayout);

        public override void Setup()
        {
            Traceable.Add("default", TraceFilters.All, new ConsoleLogger { UseLock = true });
            Traceable.Add("json", TraceFilters.All, new ConsoleLogger(new LogLayout("{@=>json}")) { UseLock = true });
            Traceable.Add("jsonIndented", TraceFilters.All, new ConsoleLogger(new LogLayout("{@=>json,indent}")) { UseLock = true });
            defaultTracer = Traceable.Get("default");
            jsonTracer = Traceable.Get("json");
            jsonIndentedTracer = Traceable.Get("jsonIndented");
        }

        Tracer defaultTracer;
        Tracer jsonTracer;
        Tracer jsonIndentedTracer;

        public override Task ShowAsync()
        {
            defaultTracer.Information("default");
            jsonTracer.Information("json");
            jsonIndentedTracer.Information("jsonIndented");

            return Task.CompletedTask;
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
