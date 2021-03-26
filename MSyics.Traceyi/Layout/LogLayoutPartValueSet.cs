using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    internal class LogLayoutPartValueSet
    {
        [JsonExtensionData]
        public IDictionary<string, object> Items { get; set; }
    }
}
