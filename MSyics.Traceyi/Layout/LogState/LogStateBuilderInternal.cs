using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    internal class LogStateBuilderInternal : ILogStateBuilder
    {
        private readonly Dictionary<string, object> members = new();

        public ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembersOfTraceEvent members = LogStateMembersOfTraceEvent.All) =>
            Set("action", e.Action, members.HasFlag(LogStateMembersOfTraceEvent.Action), false).
            Set("traced", e.Traced, members.HasFlag(LogStateMembersOfTraceEvent.Traced)).
            Set("elapsed", e.Elapsed, members.HasFlag(LogStateMembersOfTraceEvent.Elapsed)).
            SetNullable("activityId", e.ActivityId, members.HasFlag(LogStateMembersOfTraceEvent.ActivityId)).
            SetNullable("scopeLabel", e.ScopeLabel, members.HasFlag(LogStateMembersOfTraceEvent.ScopeLabel)).
            SetNullable("scopeId", e.ScopeId, members.HasFlag(LogStateMembersOfTraceEvent.ScopeId)).
            SetNullable("scopeParentId", e.ScopeParentId, members.HasFlag(LogStateMembersOfTraceEvent.ScopeParentId)).
            Set("scopeDepth", e.ScopeDepth, members.HasFlag(LogStateMembersOfTraceEvent.ScopeDepth)).
            Set("threadId", e.ThreadId, members.HasFlag(LogStateMembersOfTraceEvent.ThreadId)).
            Set("processId", e.ProcessId, members.HasFlag(LogStateMembersOfTraceEvent.ProcessId)).
            SetNullable("processName", e.ProcessName, members.HasFlag(LogStateMembersOfTraceEvent.ProcessName)).
            SetNullable("machineName", e.MachineName, members.HasFlag(LogStateMembersOfTraceEvent.MachineName)).
            SetNullable("message", e.Message, members.HasFlag(LogStateMembersOfTraceEvent.Message)).
            SetExtensions(e.Extensions, members.HasFlag(LogStateMembersOfTraceEvent.Extensions));

        public ILogStateBuilder Set<T>(string member, T value, bool enabled = true, bool ignoreWhenDefault = true) where T : struct
        {
            if (enabled)
            {
                if (ignoreWhenDefault)
                {
                    if (!value.Equals(default(T)))
                    {
                        members[member] = value;
                    }
                }
                else
                {
                    members[member] = value;
                }
            }
            return this;
        }

        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled = true) where T : class
        {
            if (enabled && value is not null)
            {
                members[member] = value;
            }
            return this;
        }

        public ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled = true)
        {
            if (enabled)
            {
                foreach (var ex in extensions)
                {
                    members[ex.Key] = ex.Value;
                }
            }
            return this;
        }

        public LogState Build()
        {
            if (members.Count == 0) return null;
            return new() { Members = members };
        }
    }
}
