using MSyics.Traceyi.Layout;
using MSyics.Traceyi.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi.Example
{
    class UsingShiftrJIS : IExample
    {
        public Tracer Tracer { get; set; }

        public void Setup()
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

        public void Shutdown()
        {
            Traceable.Shutdown();
        }

        public void Test()
        {
            Tracer.Information("UsingShiftJIS ほげほげ");
        }
    }
}
