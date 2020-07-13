using Microsoft.Extensions.Logging;

namespace MSyics.Traceyi
{
    /// <summary>
    /// TraceyiLogger を提供します。
    /// </summary>
    [ProviderAlias("Traceyi")]
    public class TraceyiLoggerProvider : ILoggerProvider
    {
        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return new TraceyiLogger(Traceable.Get(categoryName));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Traceable.Shutdown();
        }
    }
}