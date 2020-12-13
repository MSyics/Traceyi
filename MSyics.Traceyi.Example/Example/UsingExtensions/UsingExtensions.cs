using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingExtensions : Example
    {
        public override string Name => nameof(UsingExtensions);

        private ILogger logger;

        public override void Setup()
        {
            logger = LoggerFactory.Create(x =>
            {
                x.ClearProviders();
                x.AddTraceyi(@"Example\UsingExtensions\traceyi.json");
            }).CreateLogger<UsingExtensions>();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override Task ShowAsync()
        {
            using (logger.BeginScope("{operationId}", nameof(ShowAsync)))
            {
                logger.LogInformation("test {test}", 100);
            }

            return Task.CompletedTask;
        }
    }
}
