using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    internal class ExceptionToStringJsonConverter : JsonConverter<Exception>
    {
        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsClass) { return false; }
            
            var type = typeToConvert;
            do
            {
                if (type == typeof(Exception)) { return true; }
                type = type.BaseType;
            } while (type != null);

            return false;
        }
    }
}
