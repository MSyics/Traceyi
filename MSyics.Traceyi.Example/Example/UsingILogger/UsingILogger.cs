using Microsoft.Extensions.Logging;

namespace MSyics.Traceyi;

class UsingILogger : Example
{
    ILogger logger;

    public override string Name => nameof(UsingILogger);

    public override void Setup()
    {
        logger = LoggerFactory.
            Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddTraceyi(@"Example\UsingILogger\traceyi.json");
            }).
            CreateLogger<UsingILogger>();
    }

    public override void Teardown()
    {
        Traceable.Shutdown();
    }

    public override async Task ShowAsync()
    {
        logger.GetContext().ActivityId = "AAA";
        using var _ = logger.BeginScope(label: Name);

        await Task.Run(() =>
        {
            logger.LogCritical(new EventId(12345, "hogehoge"), "abc");

            logger.LogInformation("test:{test}", x => x.test = new[] { "hogehoge", "piyopiyo" });

            logger.LogInformation("test:{test}", 1);
            logger.LogCritical("test:{test}", x => x.test = 2);
            logger.LogWarning(3, "test:{test}", 3);
            logger.LogError(x => x.test = 4);
            try
            {
                File.Open("", FileMode.Open);
            }
            catch (Exception ex)
            {
                logger.LogTrace(ex, "test:{test}", 5);
            }

            try
            {
                File.Open("", FileMode.Open);
            }
            catch (Exception ex)
            {
                logger.LogDebug(6, ex, "test:{test}", x => x.test = 6);
            }
        });
    }
}
