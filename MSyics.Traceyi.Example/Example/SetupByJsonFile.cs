using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByJsonFile : ITrecyiExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            Traceable.Add("Traceyi.json");

            Tracer = Traceable.Get("messageOnly");
        }

        public void Test()
        {
            Tracer.Information("SetupByJsonFile");
        }
    }
}
