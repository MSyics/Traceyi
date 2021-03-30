using System;
using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    public interface ILogStateBuilder
    {
        LogState Build();
        ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembers members = LogStateMembers.All);
        ILogStateBuilder Set<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct;
        ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class;
        ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled);
    }

    public class LogStateBuilder : ILogStateBuilder
    {
        /// <summary>
        /// トレースイベントデータを記録データに設定します。
        /// </summary>
        public ILogStateBuilder SetEvent(TraceEventArgs e, LogStateMembers members = LogStateMembers.All) =>
            new InternalLogStateBuilder().SetEvent(e, members);

        /// <summary>
        /// 指定した値を記録データに設定します。
        /// </summary>
        public ILogStateBuilder Set<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct =>
            new InternalLogStateBuilder().Set(member, value, enabled, ignoreWhenDefault);

        /// <summary>
        /// Null 許容型の値を記録データに設定します。
        /// </summary>
        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class =>
            new InternalLogStateBuilder().SetNullable(member, value, enabled);

        /// <summary>
        /// 拡張データを記録データに設定します。
        /// </summary>
        public ILogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled) =>
            new InternalLogStateBuilder().SetExtensions(extensions, enabled);

        /// <summary>
        /// 記録データを構築します。
        /// </summary>
        public LogState Build() => new InternalLogStateBuilder().Build();
    }

    /// <summary>
    /// 記録データを構築します。
    /// </summary>
    internal class InternalLogStateBuilder : ILogStateBuilder
    {
        private readonly Dictionary<string, object> members = new();

        /// <summary>
        /// トレースイベントデータを記録データに設定します。
        /// </summary>
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

        /// <summary>
        /// 指定した値を記録データに設定します。
        /// </summary>
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

        /// <summary>
        /// Null 許容型の値を記録データに設定します。
        /// </summary>
        public ILogStateBuilder SetNullable<T>(string member, T value, bool enabled) where T : class
        {
            if (enabled && value is not null)
            {
                members[member] = value;
            }
            return this;
        }

        /// <summary>
        /// 拡張データを記録データに設定します。
        /// </summary>
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

        /// <summary>
        /// 記録データを構築します。
        /// </summary>
        public LogState Build()
        {
            if (members.Count == 0) return null;
            return new() { Members = members };
        }
    }
}
