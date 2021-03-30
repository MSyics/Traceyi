using System.Collections.Generic;
using System.Dynamic;

namespace MSyics.Traceyi
{
    internal sealed class DictionaryedDynamicObject : DynamicObject
    {
        public readonly Dictionary<string, object> Items = new();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Items.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Items[binder.Name] = value;
            return true;
        }
    }
}
