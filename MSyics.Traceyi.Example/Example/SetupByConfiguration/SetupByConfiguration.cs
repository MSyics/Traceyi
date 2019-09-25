using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class SetupByConfiguration : Example
    {
        public override string Name => nameof(SetupByConfiguration);

        public override void Setup()
        {
            Traceable.Add(
                new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile(@"Example\SetupByConfiguration\traceyi.json", false, true).Build());
            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            using (Tracer.Scope())
            {
                Tracer.Information(Name);
            }

            return Task.CompletedTask;
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
