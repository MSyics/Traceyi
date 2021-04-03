using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingArchive : Example
    {
        public override string Name => nameof(UsingArchive);

        public override void Setup()
        {
            Traceable.Add(@"example\UsingArchive\traceyi.json");
            Tracer = Traceable.Get();
            SecondTracer = Traceable.Get("second");
        }

        Tracer SecondTracer;
        readonly Stopwatch sw = new();

        public override async Task ShowAsync()
        {
            sw.Start();

            using (Tracer.Scope(1))
            {
                for (int i = 0; i < 5; i++)
                {
                    await Task.Delay(1000);

                    using (Tracer.Scope())
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            await Task.WhenAll(
                                Task.Run(() => Tracer.Information("test")),
                                Task.Run(() => SecondTracer.Debug("test")));
                        }
                    }
                }
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
