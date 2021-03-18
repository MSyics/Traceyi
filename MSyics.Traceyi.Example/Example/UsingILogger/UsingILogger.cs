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
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddTraceyi(@"Example\UsingILogger\traceyi.json"));
            logger = loggerFactory.CreateLogger<UsingILogger>();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }

        public override async Task ShowAsync()
        {
            var ex = new NullReferenceException("hogehoge", new NullReferenceException("piyopiyo"));

            using (logger.BeginScope())
            //using (logger.BeginScope("{hoge} {piyo|_,4:R}", "1", 2))
            //using (logger.BeginScope("{hoge} {piyo|_,4:R}", x =>
            //{
            //    x.hoge = "1";
            //    x.piyo = 2;
            //}, "op1"))
            //using (logger.BeginScope(x =>
            //{
            //    x.hoge = "2";
            //    x.piyo = 2;
            //}, "op2"))
            using (logger.BeginScope(x =>
            {
                x.hoge = "3";
                x.piyo = 2;
            }, "operation"))
            {
                //logger.LogInformation("{hoge=>json} {piyo|_,4:R}", "1", 2);
                //logger.LogInformation(ex, "");
                //logger.LogInformation(1, "");
                //logger.LogInformation(ex, "{hoge=>json} {piyo|_,4:R} {hoge}", x =>
                //{
                //    x.hoge = "4";
                //    x.piyo = 2;
                //});
                logger.LogInformation(1, "{hoge}, piyo:{piyo|_,4:R}, {hoge=>json}", x =>
                {
                    x.hoge = 1;
                    x.piyo = 2;
                    x.p = TraceAction.Trace;
                    x.piyo2 = DateTime.Now;
                    x.fuga = TimeSpan.FromMinutes(1);
                    x.ex = ex.ToString();
                    x.ex2 = ex;
                });
                logger.LogInformation(ex, "hogehoge");
            }

            await Task.CompletedTask;
        }
    }
}
