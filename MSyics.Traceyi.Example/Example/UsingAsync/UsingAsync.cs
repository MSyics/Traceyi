using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
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

        public override async Task ShowAsync()
        {
            await Task.
                WhenAll(Enumerable.
                Range(1, 1000).
                Select(x => Task.
                Run(() => Hoge(x))).
                ToArray());

            //var result = Parallel.For(0, 100, i =>
            //{
            //    Parallel.For(0, 10, j =>
            //    {
            //        Hoge($"{i} {j}");
            //    });
            //});
            //await Task.CompletedTask;
        }

        public void Hoge(object obj)
        {
            using (Tracer.Scope())
            {
                Tracer.Information(obj);
            }
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
