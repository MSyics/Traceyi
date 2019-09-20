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
            Traceable.Add(@"example\UsingAsync\traceyi.json");
            Tracer = Traceable.Get();
        }

        public void Show()
        {
            Parallel.For(1, 10, i =>
            {
                Parallel.For(1, 10, j =>
                {
                    Hoge($"{i} {j}");
                });
            });

        }

        public void Hoge(object obj)
        {
            using (Tracer.Scope())
            {
                Tracer.Information(obj);
            }
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
