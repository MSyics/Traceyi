namespace Microsoft.Extensions.Logging
{
    using MSyics.Traceyi;

    /// <summary>
    /// TraceyiLogger を提供します。
    /// </summary>
    [ProviderAlias("Traceyi")]
    public class TraceyiLoggerProvider : ILoggerProvider
    {
        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName) => new TraceyiLogger(Traceable.Get(categoryName));

        /// <inheritdoc/>
        public void Dispose() => Traceable.Shutdown();
    }
}