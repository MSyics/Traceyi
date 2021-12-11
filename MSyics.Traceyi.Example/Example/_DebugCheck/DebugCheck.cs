namespace MSyics.Traceyi;

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
        var person = new Person { Name = "hogehoge", Age = 20 };

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

        //    using (Tracer.Scope(label: 1))
        //    {
        //        Tracer.Information(100);
                Tracer.Information(person, x => x.person = 100);
                Tracer.Information("abc");
                Tracer.Information("{person}", x => x.person = 100);
        //    }
        //}

        //var _ = Scope(0).Take(5).ToArray();

        //Tracer.Information(person);
        //Tracer.Information("{person=>json}", ex => ex.person = person);
        //Tracer.Information(ex => ex.person = person);

        return Task.CompletedTask;
    }

    private IEnumerable<int> Scope(int count)
    {
        using (Tracer.Scope(label: $"{count:00000}"))
        {
            Tracer.Start($"{count:00000}-1");
            Tracer.Start($"{count:00000}-2");
            Tracer.Start($"{count:00000}-3");
            yield return count;

            foreach (var item in Scope(++count))
            {
                yield return item;
            }
        }
    }
}

internal class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}