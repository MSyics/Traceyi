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
            var result = Parallel.For(0, 3, i =>
            {
                Parallel.For(0, 3, j =>
                {
                    Hoge($"{i} {j}");
                });
            });
            await Task.CompletedTask;
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
