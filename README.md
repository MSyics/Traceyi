# Traceyi
Traceyi is an asynchronous logging library for .NET

[![Nuget](https://img.shields.io/nuget/v/Traceyi)](https://www.nuget.org/packages/Traceyi)
[![Nuget](https://img.shields.io/nuget/vpre/Traceyi)](https://www.nuget.org/packages/Traceyi)

## Using Tracer
```csharp
class Program
{
    static void Main(string[] args)
    {
        Traceable.Add("traceyi.json");
        var tracer = Traceable.Get();
        using (tracer.Scope())
        {
            tracer.Trace(Name);
            tracer.Debug(Name);
            tracer.Information(Name);
            tracer.Warning(Name);
            tracer.Error(Name);
            tracer.Critical(Name);
        }
        Traceable.Shutdown();
    }
}
```

## Using ILogger
```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddTraceyi(context.Configuration);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```
