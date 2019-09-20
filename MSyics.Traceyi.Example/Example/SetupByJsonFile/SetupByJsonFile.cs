using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByJsonFile : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(SetupByJsonFile);

        public void Setup()
        {
            Traceable.Add(@"example\SetupByJsonFile\traceyi.json");
            Tracer = Traceable.Get();
        }

        public void Show()
        {
            using (Tracer.Scope())
            {
                Tracer.Information(Name);
            }
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
