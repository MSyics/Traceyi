using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class UsingAsync : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(UsingAsync);

        public void Setup()
        {
            Traceable.Add(@"config\settings.json");
            Tracer = Traceable.Get();
        }

        public void Show()
        {
            Fuga().Wait();
        }

        public void Hoge(object obj)
        {
            using (Tracer.Scope())
            {
                Tracer.Information(obj);
                //await Task.Delay(1000);
            }
        }

        public async Task Piyo()
        {
            var tasks = Enumerable.Range(1, 100).Select(x => Task.Run(() => Hoge(x))).ToArray();
            await Task.WhenAll(tasks);
        }

        public async Task Fuga()
        {
            using (Tracer.Scope())
            {
                var tasks = Enumerable.Range(1, 10).Select(x => Piyo()).ToArray();
                await Task.WhenAll(tasks);
            }
        }



        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
