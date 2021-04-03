using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingScope : Example
    {
        public override string Name => nameof(UsingScope);

        public override void Setup()
        {
            Traceable.Add(@"example\UsingScope\traceyi.json");
            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            Tracer.Information("out of scope");

            using (Tracer.Scope(label: nameof(ShowAsync)))
            { 
                Method_001();
            }
            Tracer.Stop("out of scope");

            return Task.CompletedTask;
        }

        private void Method_001()
        {
            using var _ = Tracer.Scope(label: nameof(Method_001));

            Method_002();

            Tracer.Stop();
            Tracer.Start(label: $"{nameof(Method_001)}`");
        }

        private void Method_002()
        {
            using var _ = Tracer.Scope(label: nameof(Method_002));
            Method_003();
        }

        private void Method_003()
        {
            using var _ = Tracer.Scope(label: nameof(Method_003));
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
