using System;
using MSyics.Traceyi;
using System.IO;

namespace MSyics.Traceyi.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var scope = TracerManager.Default.Scope())
            //{
            //    TracerManager.Default.Information(Directory.GetCurrentDirectory());
            //    TracerManager.Default.Information($"{scope}");
            //    TracerManager.Default.Information("hogehoge");
            //}

            Tracer.Information("test");
            Tracer.Context.ActivityId = "0123456";
            using (Tracer.Scope())
            {
                Tracer.Information("test");
                Tracer.Information("test");
                Tracer.Information("test");
                Tracer.Information("test");
            }
        }

        static Tracer Tracer = Traceable.Get();
    }

    static class TracerManager
    {
        public static Tracer Default = new Tracer().Settings(x =>
        {
            x.SetLog(new ConsoleLog());
            x.SetProperty("", TraceFilters.All);
        });
    }
}
