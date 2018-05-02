using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace MSyics.Traceyi.Example
{
    class _ : IExample
    {
        public Tracer Tracer { get; set; }
        public Tracer Tracer2 { get; set; }

        public void Setup()
        {
            Traceable.Add("test.json");
            //Traceable.Add("", TraceFilters.All, true, new AsyncListener<ConsoleLogger>() { Listener = new ConsoleLogger() { UseLock = false } });
            Tracer = Traceable.Get();
            //Tracer2 = Traceable.Get("test");
        }

        public void Test()
        {
            var sw = Stopwatch.StartNew();
            using (Tracer.Scope())
            {
                sw.Start();
                foreach (var item in Enumerable.Range(1, 10000))
                {
                    Tracer.Information(item);
                }
                sw.Stop();
            }
            Console.WriteLine($"sw {sw.Elapsed}");
            return;

            //for (int k = 0; k < 100; k++)
            {
                using (Tracer.Scope())
                {
                    var tasks = new Task[1000];
                    {
                        for (int i = 0; i < tasks.Length; i++)
                        {
                            tasks[i] = Task.Run(() =>
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    Tracer.Information($"1 {j}");
                                }
                            });
                        }
                    }
                    var tasks2 = new Task[1000];
                    {
                        for (int i = 0; i < tasks2.Length; i++)
                        {
                            tasks2[i] = Task.Run(() =>
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    Tracer.Information($"1 {j}");
                                }
                            });
                        }
                    }

                    Task.WaitAll(tasks.Concat(tasks2).ToArray());
                }
            }
        }
    }
}
