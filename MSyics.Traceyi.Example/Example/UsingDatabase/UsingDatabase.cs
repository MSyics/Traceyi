using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Configration;
using MSyics.Traceyi.Listeners;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;

namespace MSyics.Traceyi;

class UsingDatabase : Example
{
    ILogger logger;
    readonly Stopwatch s_sw = new();

    public override string Name => nameof(UsingDatabase);

    public override void Setup()
    {
        s_sw.Start();

        logger = LoggerFactory.
            Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddTraceyi(@"Example\UsingDatabase\traceyi.json", usable => usable.In<UsingDatabaseLoggerElement>("UsingDatabase"));
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
        sw.Start();

        await Show001();
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }

    public async Task Show001()
    {
        using (logger.BeginScope())
        {
            await Task.Delay(1000 * 2);
            await Task.WhenAll(Enumerable.Range(1, 111).Select(i =>
            {
                return Show002(111);
            }));
        }
        await Task.Delay(1000 * 2);
    }

    private Task Show002(int count)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < count; i++)
            {
                logger.LogInformation("{i}", i);
            }
        });
    }
}

class UsingDatabaseLoggerElement : ChunkTraceEventListenerElement
{
    public override ITraceEventListener GetRuntimeObject()
    {
        return new UsingDatabaseLogger(Demux, Chunk)
        {
            UseAsync = UseAsync,
            UseLock = UseLock,
            ChunkTimeout = ChunkTimeout,
        };
    }
}

class UsingDatabaseLogger : ChunkTraceEventListener
{
    readonly SQLiteConnectionStringBuilder connectionStringBuilder = new()
    {
        DataSource = @"Example\UsingDatabase\using_database.db",
        JournalMode = SQLiteJournalModeEnum.Wal,
        SyncMode = SynchronizationModes.Off,
        Pooling = true,
    };

    private DbConnection OpenDbConnection()
    {
        var cnn = new SQLiteConnection(connectionStringBuilder.ToString());
        cnn.Open();
        return cnn;
    }

    public UsingDatabaseLogger(int demux = 1, int chunk = 1) : base(demux, chunk)
    {
        using var cnn = OpenDbConnection();
        using var cmd = cnn.CreateCommand();

        cmd.CommandText = "CREATE TABLE IF NOT EXISTS logs (action TEXT, traced TEXT, elapsed TEXT)";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "DELETE FROM logs";
        cmd.ExecuteNonQuery();
    }

    protected override void WriteCore(IEnumerable<TraceEventArgs> items, int index)
    {
        using var cnn = OpenDbConnection();
        using var trn = cnn.BeginTransaction();

        foreach (var e in items)
        {
            using var cmd = cnn.CreateCommand();
            cmd.CommandText = $"INSERT INTO logs VALUES ('{e.Action}', '{e.Traced}', '{e.Elapsed}')";
            cmd.ExecuteNonQuery();
        }

        trn.Commit();
    }
}
