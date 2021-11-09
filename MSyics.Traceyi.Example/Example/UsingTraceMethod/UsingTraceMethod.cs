namespace MSyics.Traceyi;

class UsingTraceMethod : Example
{
    public override string Name => nameof(UsingTraceMethod);

    public override void Setup()
    {
        Traceable.Add(@"Example\UsingTraceMethod\traceyi.json");
        Tracer = Traceable.Get();
    }

    public override void Teardown()
    {
        Traceable.Shutdown();
    }

    public override Task ShowAsync()
    {
        using (Tracer.Scope(label: Name))
        {
            Tracer.Start(Name);

            Tracer.Trace(Name);
            Tracer.Debug(Name);
            Tracer.Information(Name);
            Tracer.Warning(Name);
            Tracer.Error(Name);
            Tracer.Critical(Name);

            Tracer.Stop(Name);
        }

        return Task.CompletedTask;
    }
}
