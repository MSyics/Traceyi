using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 記録データを構築します。
    /// </summary>
    public class LogStateBuilder : ILogStateBuilder
    {
        public ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembersOfTraceEvent members = LogStateMembersOfTraceEvent.All) =>
            new LogStateBuilderInternal().SetEvent(e, members);

        public ILogStateBuilder Set<T>(string member, T value, bool enabled = true, bool ignoreWhenDefault = true) where T : struct =>
            new LogStateBuilderInternal().Set(member, value, enabled, ignoreWhenDefault);

        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled = true) where T : class =>
            new LogStateBuilderInternal().SetNullable(member, value, enabled);

        public ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled = true) =>
            new LogStateBuilderInternal().SetExtensions(extensions, enabled);

        public LogState Build() => new LogStateBuilderInternal().Build();
    }
}
