using System.Dynamic;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi;

internal sealed class DictionaryedDynamicObject : DynamicObject
{
    [JsonExtensionData]
    public readonly Dictionary<string, object> Members = new();

    public override bool TryGetMember(GetMemberBinder binder, out object result) => Members.TryGetValue(binder.Name, out result);

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        Members[binder.Name] = value;
        return true;
    }
}
