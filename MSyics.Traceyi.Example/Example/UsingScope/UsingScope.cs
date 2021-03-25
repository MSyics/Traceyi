using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
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
            using (Tracer.Scope("start", operationId: nameof(ShowAsync)))
            { 
                Method001();
            }
            Tracer.Stop("-");

            return Task.CompletedTask;
        }

        private void Method001()
        {
            using var ts = Tracer.Scope("start", operationId: nameof(Method001));

            Method002();

            Tracer.Stop("001");
            Tracer.Start("001", operationId: $"{nameof(Method001)}`");

        }

        private void Method002()
        {
            using var ts = Tracer.Scope("start", operationId: nameof(Method002));

            Tracer.Information("002");
            Method003();
        }

        private void Method003()
        {
            using var ts = Tracer.Scope("start", operationId: nameof(Method003));

            Tracer.Information("003");
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
