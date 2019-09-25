using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingShiftrJIS : Example
    {
        public override string Name => nameof(UsingShiftrJIS);

        public override void Setup()
        {
            // Install-Package System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Traceable.Add(
                listeners: new ConsoleLogger()
                {
                    Encoding = Encoding.GetEncoding("Shift-JIS"),
                    Layout = new LogLayout("{message}"),
                });

            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            Tracer.Information("UsingShiftJIS ほげほげ");
            Tracer.Information("UsingShiftJIS ほげほげ");

            return Task.CompletedTask;
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
