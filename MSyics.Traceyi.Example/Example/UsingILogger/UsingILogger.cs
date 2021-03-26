using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
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
            logger.LogInformation("hogehoge");
            using var _ = logger.BeginScope(label: "o000");
            using (logger.BeginScope(x =>
            {
            }))
            {
                await Task.Run(() =>
                {
                    logger.GetContext().ActivityId = "a002";
                    logger.LogInformation("hogehoge");
                    logger.LogCritical("hogehoge");
                    logger.LogWarning("hogehoge");
                    logger.LogError("hogehoge");
                    logger.LogTrace(null, x => { });
                    logger.LogDebug("hogehoge", x =>
                    {
                        x.a = 1;
                        x.b = 2;
                        x.c = null;
                    });
                    logger.LogDebug(x =>
                    {
                        x.a = 1;
                        x.b = 2;
                        x.c = null;
                    });
                    logger.LogInformation(new ApplicationException("hogehoge"));

                    try
                    {
                        File.Open("hoge", FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex);
                        logger.LogError(ex, "error");
                    }
                });
            }


            await Task.CompletedTask;
        }
    }
}
