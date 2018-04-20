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
    class Program : ExampleTester
    {
        static void Main(string[] args)
        {
            new Program()
                .AddExample<SetupByManual>()
                .AddExample<SetupByConfiguration>()
                .AddExample<SetupByJsonFile>()
                .AddExample<UsingTraceMethod>()
                .AddExample<UsingShiftrJIS>()
                .AddExample<UsingCustomTraceListener>()
                .AddExample<UsingScope>()

                //.AddExample<_>()

                .Execute();
        }
    }

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

    interface ITrecyiExample
    {
        Tracer Tracer { get; set; }
        void Setup();
        void Test();
    }
}
