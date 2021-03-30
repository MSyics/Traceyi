using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    internal class LogStateBuilder
    {
        private readonly Dictionary<string, object> members = new();

        public LogStateBuilder SetValue<T>(string member, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct
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

        public LogStateBuilder SetNullableValue<T>(string member, T value, bool enabled) where T : class
        {
            if (enabled && value is not null)
            {
                members[member] = value;
            }
            return this;
        }

        public LogStateBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled)
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
