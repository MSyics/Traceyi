using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByJsonFile : IExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            Traceable.Add(@"config\settings.json");

            Tracer = Traceable.Get();
        }

        public void Shutdown()
        {
            Traceable.Shutdown();
        }

        public void Test()
        {
            Tracer.Information("SetupByJsonFile");
        }
    }
}
