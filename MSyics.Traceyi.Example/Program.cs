using System;
using MSyics.Traceyi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MSyics.Traceyi.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var scope = TracerManager.Default.Scope())
            //{
            //    TracerManager.Default.Information(Directory.GetCurrentDirectory());
            //    TracerManager.Default.Information($"{scope}");
            //    TracerManager.Default.Information("hogehoge");
            //}

            var pg = new Program();
            pg.Case1();
            pg.Case2();
            pg.Case3();
        }

        private Tracer Tracer { get; } = Traceable.Get();

        private void Case1()
        {
            Tracer.Information("hogehoge");
            Tracer.Debug("hogehoge");
            Tracer.Warning("hogehoge");
            Tracer.Error("hogehoge");
            Tracer.Start();
            Tracer.Stop();
            using (Tracer.Scope())
            {
            }
        }

        private void Case2()
        {
            using (Tracer.Scope())
            {
                var act5 = new Action(() => Tracer.Information("hogehoge"));
                var act4 = new Action(() => { using (Tracer.Scope()) { act5(); } });
                var act3 = new Action(() => { using (Tracer.Scope()) { act4(); } });
                var act2 = new Action(() => { using (Tracer.Scope()) { act3(); } });
                var act1 = new Action(() => { using (Tracer.Scope()) { act2(); } });
                act1();
            }
        }

        private void Case3()
        {
            Tracer.OnTrace += (sender, e) =>
            {
                Console.WriteLine(e.Message);
            };

            Tracer.Information("hogehoge");
        }
    }


}
