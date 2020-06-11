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
            using (Tracer.Scope(1))
            {
                Hoge();
            }
            return Task.CompletedTask;
        }

        private void Hoge()
        {
            using (Tracer.Scope(2))
            {
                Tracer.Start("2-1", "scope");
                Piyo();
                Tracer.Start("2-2", "scope");
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
