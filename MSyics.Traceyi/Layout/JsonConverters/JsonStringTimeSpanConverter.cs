using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    public class JsonStringTimeSpanConverter : JsonConverter<TimeSpan>
    {
        readonly string format;

        public JsonStringTimeSpanConverter(string format = "d\\.hh\\:mm\\:ss\\.fffffff") =>
            this.format = format;

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) => 
            writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));
    }
}
