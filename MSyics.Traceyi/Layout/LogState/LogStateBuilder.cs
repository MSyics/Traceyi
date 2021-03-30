using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 記録データを構築します。
    /// </summary>
    public class LogStateBuilder : ILogStateBuilder
    {
        /// <inheritdoc/>
        public ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembers members = LogStateMembers.All) =>
            new InternalLogStateBuilder().SetEvent(e, members);

        /// <inheritdoc/>
        public ILogStateBuilder Set<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct =>
            new InternalLogStateBuilder().Set(member, value, enabled, ignoreWhenDefault);

        /// <inheritdoc/>
        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class =>
            new InternalLogStateBuilder().SetNullable(member, value, enabled);

        /// <inheritdoc/>
        public ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled) =>
            new InternalLogStateBuilder().SetExtensions(extensions, enabled);

        /// <inheritdoc/>
        public LogState Build() => new InternalLogStateBuilder().Build();
    }
}
