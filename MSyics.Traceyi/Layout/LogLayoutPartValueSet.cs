using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    internal class LogState
    {
        [JsonExtensionData]
        public IDictionary<string, object> Members { get; set; }
    }
}
