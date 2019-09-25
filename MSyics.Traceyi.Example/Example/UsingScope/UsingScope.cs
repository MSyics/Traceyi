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
                Piyo();
            }
        }

        private void Piyo()
        {
            using (Tracer.Scope(3))
            {
                Fuga();
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
