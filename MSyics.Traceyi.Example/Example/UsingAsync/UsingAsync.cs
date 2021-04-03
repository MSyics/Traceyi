using System;
using System.Diagnostics;
using System.Linq;
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

        private readonly Stopwatch sw = new Stopwatch();

        public override async Task ShowAsync()
        {
            sw.Start();

            Tracer.Context.ActivityId = "A1";
            Tracer.Information("{i}", x => x.i = 100);

            using (Tracer.Scope("{i}", x => x.i = 1))
            {
                await Task.
                    WhenAll(Enumerable.
                    Range(1, 10000).
                    Select(x => Hoge(x)).
                    ToArray());

                Tracer.Information("{i}", x => x.i = 1);
                Tracer.Start("{i}", x => x.i = 1.1);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public async Task Hoge(object obj)
        {
            Tracer.Context.ActivityId = "A2";
            using (Tracer.Scope("{i}", x => x.i = 2))
            {
                await Task.Run(() =>
                {
                    Tracer.Context.ActivityId = "A3";
                    using (Tracer.Scope("{i}", x => x.i = 3))
                    {
                        Tracer.Information("3 {i}", x => x.i = obj);
                    }
                });

                Tracer.Information("2 {i}", x => x.i = obj);
                Tracer.Start("{i}", x => x.i = 2.2);
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
