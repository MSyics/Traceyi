using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class DebugCheck : Example
    {
        public override string Name => nameof(DebugCheck);

        public override void Setup()
        {
            Traceable.Add(@"Example\_DebugCheck\traceyi.json");
            Tracer = Traceable.Get();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override Task ShowAsync()
        {
            //Tracer.Information($"hogehoge");
            //using (Tracer.Scope(label: Name))
            //{
            //    Tracer.Start(Name);
            //    Tracer.Trace(Name);
            //    Tracer.Debug(Name);
            //    Tracer.Information(Name);
            //    Tracer.Warning(Name);
            //    Tracer.Error(Name);
            //    Tracer.Critical(Name);
            //    Tracer.Stop(Name);
            //}

            var _ = Scope(0).Take(10).ToArray();

            return Task.CompletedTask;
        }

        private IEnumerable<int> Scope(int count)
        {
            using (Tracer.Scope(label: $"{count:00000}"))
            {
                yield return count;

                foreach (var item in Scope(++count))
                {
                    yield return item;
                }
            }
        }
    }
}
