namespace MSyics.Traceyi;

class UsingScope : Example
{
    public override string Name => nameof(UsingScope);

    public override void Setup()
    {
        Traceable.Add(@"example\UsingScope\traceyi.json");
        Tracer = Traceable.Get();
    }

    public override Task ShowAsync()
    {
        Tracer.Information("out of scope");

        using (Tracer.Scope(label: nameof(ShowAsync)))
        {
            FireOne();
        }
        Tracer.Stop("out of scope");

        return Task.CompletedTask;
    }

    private void FireOne()
    {
        using var _ = Tracer.Scope(label: nameof(FireOne));

        FireTwo();

        Tracer.Stop();
        Tracer.Start(label: $"{nameof(FireOne)}`");
    }

    private void FireTwo()
    {
        using var _ = Tracer.Scope(label: nameof(FireTwo));
        FireThree();
    }

    private void FireThree()
    {
        using var _ = Tracer.Scope(label: nameof(FireThree));
    }

    public override void Teardown()
    {
        Traceable.Shutdown();
    }
}
