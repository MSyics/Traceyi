using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingAsync : Example
    {
        public override string Name => nameof(UsingAsync);

        public override void Setup()
        {
            Traceable.Add(@"example\UsingAsync\traceyi.json");
            Tracer = Traceable.Get();
        }

        private Stopwatch sw = new Stopwatch();

        public override async Task ShowAsync()
        {
            sw.Start();
            Tracer.Information(100);

            using (Tracer.Scope(1))
            {
                await Task.
                    WhenAll(Enumerable.
                    Range(1, 10000).
                    Select(x => Hoge(x)).
                    ToArray());

                Tracer.Information(1);
                Tracer.Start(1.1);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public async Task Hoge(object obj)
        {
            using (Tracer.Scope(2))
            {
                await Task.Run(() =>
                 {
                     using (Tracer.Scope(3))
                     {
                         Tracer.Information($"{3} {obj}");
                     }
                 });

                Tracer.Information($"{2} {obj}");
                Tracer.Start(2.2);
            }
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
