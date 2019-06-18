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

        public string Name => nameof(UsingShiftrJIS);

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

        public void Show()
        {
            Tracer.Information("UsingShiftJIS ほげほげ");
        }

        public void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
