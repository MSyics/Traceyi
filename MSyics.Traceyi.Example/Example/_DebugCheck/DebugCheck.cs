using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class DebugCheck : Example
    {
        public override string Name => nameof(DebugCheck);

        public override void Setup()
        {
            Traceable.Add(@"Example\_DebugCheck\traceyi.json");
            Tracer = Traceable.Get();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override Task ShowAsync()
        {
            Tracer.Information($"hogehoge");
            using (Tracer.Scope(label: Name))
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

            return Task.CompletedTask;
        }
    }
}
