using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingExtensions : Example
    {
        public override string Name => nameof(UsingExtensions);

        public override void Setup()
        {
            Traceable.Add(@"example\UsingExtensions\traceyi.json");
            Tracer = Traceable.Get();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override Task ShowAsync()
        {
            using var scope = Tracer.Scope("test:{test}", x =>
            {
                x.test = "start";
            },
            label: Name);
            
            Tracer.Information("test:{test}", x => x.test = "info");
            scope.Stop("test:{test}", x => x.test = "stop");

            return Task.CompletedTask;
        }
    }
}
