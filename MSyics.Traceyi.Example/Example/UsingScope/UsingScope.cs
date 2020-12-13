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
                Show2();
            }
            return Task.CompletedTask;
        }

        private void Show2()
        {
            using (Tracer.Scope(2, "2 start", "2 stop"))
            {
                Tracer.Start("2-1", "2-1 start");
                Show3();
                Tracer.Start("2-2", "2-2 start");
            }
        }

        private void Show3()
        {
            using (Tracer.Scope(3, "3 start", "3 stop"))
            {
                Tracer.Information("3");
                Show4();
                Tracer.Stop("Since it has not started, this stop will be ignored.");
            }
        }

        private void Show4()
        {
            using (Tracer.Scope(4, "4 start", "4 stop"))
            {
                Tracer.Information("4");
            }
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
