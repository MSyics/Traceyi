using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    class UsingTest : Example
    {
        ILogger logger;
        Stopwatch s_sw = new();

        public override string Name => nameof(UsingTest);

        public override void Setup()
        {
            logger = LoggerFactory.
                Create(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddTraceyi(@"Example\UsingTest\traceyi.json", usable => usable.In<HogeElement>("hoge"));
                }).
                CreateLogger<UsingILogger>();
        }

        public override void Teardown()
        {
            Traceable.Shutdown();
            s_sw.Stop();
            Console.WriteLine(s_sw.ElapsedMilliseconds);
        }

        public override async Task ShowAsync()
        {
            Stopwatch sw = new();
            s_sw.Start();
            sw.Start();
            using (logger.BeginScope())
            {
                await Task.WhenAll(Enumerable.Range(1, 100).Select(i =>
                {
                    return Test(10);
                }));
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private Task Test(int count)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    logger.LogInformation($"{i}");
                }
            });
        }
    }

    class HogeElement : TraceEventListenerElement
    {
        public override ITraceEventListener GetRuntimeObject()
        {
            return new HogeLogger
            {
            };
        }
    }

    class HogeLogger : TraceEventListener
    {
        readonly SQLiteConnectionStringBuilder connectionStringBuilder = new()
        {
            //DataSource = ":memory:"
            DataSource = "using_test.db",
            JournalMode = SQLiteJournalModeEnum.Wal,
            SyncMode = SynchronizationModes.Off,
            //Pooling = true,
        };

        private DbConnection OpenDbConnection()
        {
            var cnn = new SQLiteConnection(connectionStringBuilder.ToString());
            cnn.Open();
            return cnn;
        }

        public HogeLogger() : base(useLock: false, concurrency: 10)
        {
            using var cnn = OpenDbConnection();
            using var cmd = cnn.CreateCommand();
            cmd.CommandText = "select sqlite_version()";
            Console.WriteLine(cmd.ExecuteScalar());

            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS logs (" +
                " action TEXT" +
                ",traced TEXT" +
                ",elapsed TEXT" +
                ")";
            cmd.ExecuteNonQuery();
        }

        protected override void WriteCore(TraceEventArgs e, int index)
        {
            using var cnn = OpenDbConnection();
            using var trn = cnn.BeginTransaction();
            using var cmd = cnn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO logs VALUES (" +
                $"'{e.Action}','{e.Traced}','{e.Elapsed}')";
            cmd.ExecuteNonQuery();
            trn.Commit();
        }
    }
}
