using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout;

public class JsonStringIfWriteFailureConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var type = value.GetType();

        if (type.IsPrimitive)
        {
            JsonSerializer.Serialize(writer, value, options);
            return;
        }

        switch (value)
        {
            case string:
            case Enum:
            case DateTimeOffset:
            case TimeSpan:
                JsonSerializer.Serialize(writer, value, options);
                return;
            case MemberInfo:
                JsonSerializer.Serialize(writer, value.ToString(), options);
                return;
            default:
                break;
        }

        try
        {
            // 書込み確認（循環参照等）
            JsonSerializer.SerializeToUtf8Bytes(value, options);
            JsonSerializer.Serialize(writer, value, options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{value}, {ex}");
            JsonSerializer.Serialize(writer, value.ToString(), options);
        }
    }
}
