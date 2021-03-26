using System.Collections.Generic;

namespace MSyics.Traceyi.Layout
{
    internal class LogLayoutPartValueSetBuilder
    {
        private readonly Dictionary<string, object> items = new();

        public LogLayoutPartValueSetBuilder SetValue<T>(string name, T value, bool enabled, bool ignoreWhenDefault = true) where T : struct
        {
            if (enabled)
            {
                if (ignoreWhenDefault)
                {
                    if (!value.Equals(default(T)))
                    {
                        items[name] = value;
                    }
                }
                else
                {
                    items[name] = value;
                }
            }
            return this;
        }

        public LogLayoutPartValueSetBuilder SetNullableValue<T>(string name, T value, bool enabled) where T : class
        {
            if (enabled && value is not null)
            {
                items[name] = value;
            }
            return this;
        }

        public LogLayoutPartValueSetBuilder SetExtensions(IDictionary<string, object> extensions, bool enabled)
        {
            if (enabled)
            {
                foreach (var ex in extensions)
                {
                    items[ex.Key] = ex.Value;
                }
            }
            return this;
        }

        public LogLayoutPartValueSet Build()
        {
            if (items.Count == 0) return null;
            return new() { Items = items };
        }
    }
}
