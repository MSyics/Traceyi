using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Example
{
    class Class1 : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(Class1);

        public void Setup()
        {
            Traceable.Add(@"config\hoge.json");
            Tracer = Traceable.Get();
        }

        public void Show()
        {
            using (Tracer.Scope(1))
            {
                Piyo();
            }
        }

        private async void Piyo()
        {
            using (Tracer.Scope())
            {
                //Tracer.Start();
                var tasks = Enumerable.Range(1, 100).Select(i => TestAsync(i)).ToArray();
                await Task.WhenAll(tasks);
            }
        }

        private async Task TestAsync(object obj)
        {
            await Task.Run(() =>
            {
                using (Tracer.Scope())
                {
                    Tracer.Information(obj);
                }
            });
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
