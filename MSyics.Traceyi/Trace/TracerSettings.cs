using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Traceyi
{
    public sealed class TracerSettings
    {
        public string Name { get; internal set; }
        public TraceFilters Filter { get; set; } = TraceFilters.All;
        public bool UseMemberInfo { get; set; } = true;
    }
}
