using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    internal class TimeSpanToStringJsonConverter : JsonConverter<TimeSpan>
    {
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("d\\.hh\\:mm\\:ss\\.fffffff", CultureInfo.InvariantCulture));
        }

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
