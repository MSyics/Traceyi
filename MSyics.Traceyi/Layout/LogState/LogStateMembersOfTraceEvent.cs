using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// トレースイベントの記録データに使用するメンバーを表します。
    /// </summary>
    [Flags]
    public enum LogStateMembersOfTraceEvent
    {
        Action = 1,
        Traced = 1 << 2,
        Elapsed = 1 << 3,
        ActivityId = 1 << 4,
        ScopeLabel = 1 << 5,
        ScopeId = 1 << 6,
        ScopeParentId = 1 << 7,
        ScopeDepth = 1 << 8,
        ThreadId = 1 << 9,
        ProcessId = 1 << 10,
        ProcessName = 1 << 11,
        MachineName = 1 << 12,
        Message = 1 << 13,
        Extensions = 1 << 14,

        All = Action | Traced | Elapsed | ActivityId | ScopeLabel | ScopeId | ScopeParentId | ScopeDepth | ThreadId | ProcessId | ProcessName | MachineName | Message | Extensions,
        None = 1 << 100,
    }
}
