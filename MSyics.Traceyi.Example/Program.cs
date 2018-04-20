using System;
using MSyics.Traceyi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections.Generic;

namespace MSyics.Traceyi.Example
{
    abstract class ExampleTester
    {
        private List<ITrecyiExample> Examples { get; } = new List<ITrecyiExample>();
        public ExampleTester AddExample<T>() where T : ITrecyiExample
        {
            Examples.Add(Activator.CreateInstance<T>());
            return this;
        }

        public void Execute()
        {
            foreach (var item in Examples)
            {
                item.Setup();
                item.Test();
            }
        }
    }

    class Program : ExampleTester
    {
        static void Main(string[] args)
        {
            new Program()
                .AddExample<SetupByManual>()
                .AddExample<SetupByConfiguration>()
                .AddExample<SetupByJsonFile>()
                .AddExample<UsingShiftrJIS>()
                .AddExample<UsingCustomTraceListener>()
                .AddExample<UsingScope>()
                .AddExample<Hoge>()

                .Execute();



            ////using (var scope = TracerManager.Default.Scope())
            ////{
            ////    TracerManager.Default.Information(Directory.GetCurrentDirectory());
            ////    TracerManager.Default.Information($"{scope}");
            ////    TracerManager.Default.Information("hogehoge");
            ////}

            var pg = new Program();
            //pg.Case1();
            ////pg.Case2();
            ////pg.Case3();
            ////pg.Case4();
            ////Console.WriteLine("----");
            ////pg.Case4();
            ////pg.Case_();
            //pg.Case5();
        }

        private Tracer Tracer { get; } = Traceable.Get("");




        private void Case5()
        {
            using (Tracer.Scope())
            {
                var tasks = new Task[10];
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Tracer.Information(j);
                        }
                    });
                }
                Task.WaitAll(tasks);
            }
        }



        private void Case1()
        {
            Tracer.Information("テスト");
            Tracer.Debug("hogehoge");
            Tracer.Warning("hogehoge");
            Tracer.Error("hogehoge");
            Tracer.Start();
            Tracer.Stop();
        }
    }
}
