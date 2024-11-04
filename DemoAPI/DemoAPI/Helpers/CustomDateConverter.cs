using Newtonsoft.Json;
using System;
using System.Globalization;

namespace DemoAPI.Helpers
{
    public class CustomDateConverter : JsonConverter
    {
        private readonly string _format = "MM/dd/yyyy";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime?) || objectType == typeof(DateTime);
            //throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || (reader.TokenType == JsonToken.String && string.IsNullOrWhiteSpace((string)reader.Value))) 
                return null;

            string dateStr = (string)reader.Value;
            DateTime parsedDate;
            if(DateTime.TryParseExact(dateStr, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)) 
                return parsedDate;

            return null;
            //throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if(value is DateTime dateTimeValue)
            {
                writer.WriteValue(dateTimeValue.ToString(_format));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
