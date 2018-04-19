using System;
using MSyics.Traceyi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace MSyics.Traceyi.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("Traceyi.json", false, true);
            var config = builder.Build();
            Traceable.AddConfiguration(config);

            //using (var scope = TracerManager.Default.Scope())
            //{
            //    TracerManager.Default.Information(Directory.GetCurrentDirectory());
            //    TracerManager.Default.Information($"{scope}");
            //    TracerManager.Default.Information("hogehoge");
            //}

            var pg = new Program();
            //pg.Case1();
            //pg.Case2();
            //pg.Case3();
            //pg.Case4();
            //Console.WriteLine("----");
            //pg.Case4();
            //pg.Case_();
            pg.Case5();
        }

        private Tracer Tracer { get; } = Traceable.Get("");

        private void Case5()
        {
            var tasks = new Task[100000];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Tracer.Information(j);
                    }
                });
            }
            Task.WaitAll(tasks);
        }


        private void Case_()
        {
            for (int _ = 0; _ < 5; _++)
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 10000; i++)
                {
                    Tracer.Information("hogehoge");
                    //writer.WriteLine(DateTime.Now.ToString("yyyy/mm/dd"));
                    //writer.WriteLine("{0:yyyy/mm/dd}", DateTime.Now);
                    //writer.WriteLine("{0}", "hogehoge");
                    //writer.WriteLine("hogehoge");
                    //writer.
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
        }

        private void Test()
        {
            using (Tracer.Scope())
            {
                Tracer.Information("hoge");
            }
        }

        private void Case4()
        {
            Tracer.Context.ActivityId = 1;
            using (var scope = Tracer.Scope())
            {
                var tasks = Enumerable
                .Range(1, 2)
                .Select(i => Task.Factory.StartNew(() => Test()));

                Task.WaitAll(tasks.ToArray());
            }
        }


        private void Case1()
        {
            Tracer.Information("テスト");
            Tracer.Debug("hogehoge");
            Tracer.Warning("hogehoge");
            Tracer.Error("hogehoge");
            Tracer.Start();
            Tracer.Stop();
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
            using (Tracer.Scope())
            {
                Tracer.Start();
                Tracer.Start();
                Tracer.Start();
                Tracer.Start();
            }
        }
    }

    internal sealed class ReuseFileStream : FileStream
    {
        /// <summary>
        /// ReuseFileStream クラスのインスタンスを初期化します。
        /// </summary>
        public ReuseFileStream(string path)
            : base(path, FileMode.Append, FileAccess.Write, FileShare.Read)
        {
        }

        /// <summary>
        /// このメソッドの代わりに Clean メソッドを使用してください。
        /// </summary>
        public override void Close()
        {
            // StreamWriter を閉じてもストリームを閉じないようにするために、
            // ここでは何もしません。
        }

        /// <summary>
        /// 現在のストリームを閉じて関連付けられているリソースを解放します。
        /// </summary>
        public void Clean() => base.Close();

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
