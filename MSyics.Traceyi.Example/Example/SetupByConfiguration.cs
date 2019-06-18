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
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(@"config\settings.json", false, true);
            var config = builder.Build();
            Traceable.Add(config);

            Tracer = Traceable.Get();
        }

        public void Show()
        {
            Tracer.Information("SetupByConfiguration");
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
