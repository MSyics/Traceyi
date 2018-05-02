using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class SetupByConfiguration : IExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile(@"config\setByConfiguration.json", false, true);
            var config = builder.Build();
            Traceable.Add(config);

            Tracer = Traceable.Get();
        }

        public void Shutdown()
        {
        }

        public void Test()
        {
            Tracer.Information("SetupByConfiguration");
            Traceable.Shutdown();
        }
    }
}
