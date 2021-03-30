using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    internal class InternalLogStateBuilder : ILogStateBuilder
    {
        private readonly Dictionary<string, object> members = new();

        public ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembers members = LogStateMembers.All) =>
            Set("action", e.Action, members.HasFlag(LogStateMembers.Action), false).
            Set("traced", e.Traced, members.HasFlag(LogStateMembers.Traced)).
            Set("elapsed", e.Elapsed, members.HasFlag(LogStateMembers.Elapsed)).
            SetNullable("activityId", e.ActivityId, members.HasFlag(LogStateMembers.ActivityId)).
            SetNullable("scopeLabel", e.ScopeLabel, members.HasFlag(LogStateMembers.ScopeLabel)).
            SetNullable("scopeId", e.ScopeId, members.HasFlag(LogStateMembers.ScopeId)).
            SetNullable("scopeParentId", e.ScopeParentId, members.HasFlag(LogStateMembers.ScopeParentId)).
            Set("scopeDepth", e.ScopeDepth, members.HasFlag(LogStateMembers.ScopeDepth)).
            Set("threadId", e.ThreadId, members.HasFlag(LogStateMembers.ThreadId)).
            Set("processId", e.ProcessId, members.HasFlag(LogStateMembers.ProcessId)).
            SetNullable("processName", e.ProcessName, members.HasFlag(LogStateMembers.ProcessName)).
            SetNullable("machineName", e.MachineName, members.HasFlag(LogStateMembers.MachineName)).
            SetNullable("message", e.Message, members.HasFlag(LogStateMembers.Message)).
            SetExtensions(e.Extensions, members.HasFlag(LogStateMembers.Extensions));

        public ILogStateBuilder Set<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct
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

        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class
        {
            if (enabled && value is not null)
            {
                members[member] = value;
            }
            return this;
        }

        public ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled)
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
