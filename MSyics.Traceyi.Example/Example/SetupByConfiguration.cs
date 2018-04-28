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
                   .AddJsonFile("Traceyi.json", false, true);
            var config = builder.Build();
            Traceable.Add(config);

            Tracer = Traceable.Get("messageOnly");
        }

        public void Test()
        {
            Tracer.Information("SetupByConfiguration");
        }
    }
}
