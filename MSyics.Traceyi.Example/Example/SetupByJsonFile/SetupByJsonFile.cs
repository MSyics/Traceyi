using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class SetupByJsonFile : Example
    {
        public override string Name => nameof(SetupByJsonFile);

        public override void Setup()
        {
            Traceable.Add(@"example\SetupByJsonFile\traceyi.json");
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
