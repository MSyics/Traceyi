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

            Hoge hoge = new() { Id = 100 };
            hoge.Value = hoge;
            Hoge a = new() { Id = 100 };

            logger.GetContext().ActivityId = hoge;
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
                    logger.LogError("■あいうえお");
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

                    try
                    {
                        File.Open("hoge", FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "error");
                    }

                    //for (int i = 0; i < 10; i++)
                    //{
                    //    logger.LogInformation("hogehoge");
                    //}

                    logger.LogError(x => x.hoge = hoge);
                    logger.LogError(x => x.hoge = new[] { hoge });
                    logger.LogError(x => x.hoge = new[] { 1, 2 });
                    
                    IDictionary<string, object> dic = new Dictionary<string, object>
                    {
                        ["hogehoge"] = 100
                    };
                    logger.GetContext().ActivityId = a;
                    logger.LogError(x => x.hoge = null);

                    var pi = typeof(Hoge).GetProperty(nameof(Hoge.Value));
                    logger.GetContext().ActivityId = pi;
                    logger.LogError(x => x.hoge = pi);

                });
            }


            await Task.CompletedTask;
        }

        class Hoge
        {
            public int Id { get; set; }
            public Hoge Value { get; set; }
        }
    }
}
