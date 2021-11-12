using MSyics.Traceyi;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// TraceyiLogger を提供します。
/// </summary>
[ProviderAlias("Traceyi")]
public class TraceyiLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TraceyiLogger(Traceable.Get(categoryName));

    public void Dispose()
    {
        Traceable.Shutdown();
        GC.SuppressFinalize(this);
    }
}
