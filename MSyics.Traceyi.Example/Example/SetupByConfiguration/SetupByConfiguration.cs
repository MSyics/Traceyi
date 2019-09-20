using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByConfiguration : IExample
    {
        public Tracer Tracer { get; set; }

        public string Name => nameof(SetupByConfiguration);

        public void Setup()
        {
            Traceable.Add(
                new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile(@"Example\SetupByConfiguration\traceyi.json", false, true).Build());
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
