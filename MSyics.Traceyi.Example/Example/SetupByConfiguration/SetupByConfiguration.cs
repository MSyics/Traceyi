using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class SetupByConfiguration : Example
    {
        public override string Name => nameof(SetupByConfiguration);

        public override void Setup()
        {
            var configuration = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile(@"Example\SetupByConfiguration\traceyi.json", false, true).
                Build();
            Traceable.Add(configuration);
            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            using (Tracer.Scope(label: Name))
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
