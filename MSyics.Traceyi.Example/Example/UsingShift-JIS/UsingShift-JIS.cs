using MSyics.Traceyi.Listeners;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingShiftJIS : Example
    {
        public override string Name => nameof(UsingShiftJIS);

        public override void Setup()
        {
            // Install-Package System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Traceable.Add(
                listeners: new ConsoleLogger()
                {
                    Encoding = Encoding.GetEncoding("Shift-JIS"),
                });

            Tracer = Traceable.Get();
        }

        public override Task ShowAsync()
        {
            using (Tracer.Scope(label: Name))
            {
                Tracer.Information("UsingShiftJIS ほげほげ");
                Tracer.Information("UsingShiftJIS ほげほげ");
            }

            return Task.CompletedTask;
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
        }
    }
}
