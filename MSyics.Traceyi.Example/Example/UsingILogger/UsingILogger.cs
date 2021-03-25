using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingILogger : Example
    {
        public override string Name => nameof(UsingILogger);
        ILogger logger;

        public override void Setup()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddTraceyi(@"Example\UsingILogger\traceyi.json");
            });
            logger = loggerFactory.CreateLogger<UsingILogger>();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
        
        public override async Task ShowAsync()
        {
            //logger.GetContext().ActivityId = "a001";
            using var _ = logger.BeginScope(operationId: "o000");
            using (logger.BeginScope(x =>
            {
            }))
            {
                await Task.Run(() =>
                {
                    logger.GetContext().ActivityId = "a002";
                    logger.LogInformation("hogehoge");
                    logger.LogInformation("hogehoge", x =>
                    {
                        x.a = 1;
                        x.b = 2;
                        x.c = 3;
                    });
                });
            }


            await Task.CompletedTask;
        }
    }
}
