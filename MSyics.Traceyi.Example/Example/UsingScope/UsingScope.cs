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
            Tracer.Context.ActivityId = 100;

            Tracer.Information("out of scope");
            using (Tracer.Scope(1))
            {
                Hoge();
            }
            return Task.CompletedTask;
        }

        private void Hoge()
        {
            using (Tracer.Scope(2, "2 start", "2 stop"))
            {
                Tracer.Start("2-1", "2-1 start");
                Piyo();
                Tracer.Start("2-2", "2-2 start");
            }
        }

        private void Piyo()
        {
            using (Tracer.Scope(3))
            {
                Fuga();
                Tracer.Stop("stop");
            }
        }

        private void Fuga()
        {
            using (Tracer.Scope())
            {
                Tracer.Information("fugafuga");
            }
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
