using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class UsingScope2 : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(UsingScope2);

        public void Setup()
        {
            Traceable.Add(@"config\settings.json");
            Tracer = Traceable.Get();
        }

        public async void Show()
        {
            //Tracer.Context.ActivityId = 100;
            using (Tracer.Scope())
            {
                await Hoge();
            }
        }

        private async Task Hoge()
        {
            Tracer.Start();
            Piyo();
            await Task.CompletedTask;
        }

        private Task Piyo()
        {
            Tracer.Start();
            Fuga();
            return Task.CompletedTask;
        }

        private Task Fuga()
        {
            Tracer.Start();
            Tracer.Information("fugafuga");
            return Task.CompletedTask;
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
