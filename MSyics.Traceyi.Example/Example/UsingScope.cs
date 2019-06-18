using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingScope : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(UsingScope);

        public void Setup()
        {
            Traceable.Add(@"config\settings.json");
            Tracer = Traceable.Get();
        }

        public void Show()
        {
            Tracer.Context.ActivityId = 100;
            using (Tracer.Scope(1))
            {
                Hoge();
            }
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

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
