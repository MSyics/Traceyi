using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi
{
    public sealed class TracerSettings
    {
        public string Name { get; internal set; }
        public TraceFilters Filter { get; set; } = TraceFilters.All;
        public TracerListens CanListens { get; } = new TracerListens();
    }

    public sealed class TracerListens
    {
        public bool Traced { get; internal set; } = true;
        public bool Action { get; internal set; } = true;

        public bool Message { get; set; } = true;
        public bool ActivityId { get; set; } = true;
        public bool OperationId { get; set; } = true;
        public bool ClassName { get; set; } = true;
        public bool MemberName { get; set; } = true;

        public bool ThreadId { get; set; } = true;
        public bool ProcessId { get; set; } = true;
        public bool ProcessName { get; set; } = true;
        public bool MachineName { get; set; } = true;
    }
}
