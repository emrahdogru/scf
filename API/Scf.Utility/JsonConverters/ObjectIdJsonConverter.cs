using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Utility.JsonConverters
{
    public class ObjectIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(ObjectId);


        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.Equals(""))
            {
                if (objectType.IsGenericType)
                    return null;
                return ObjectId.GenerateNewId();
            }


            return ObjectId.Parse(reader.Value as string);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
            writer.WriteValue(value == null ? null : ((ObjectId)value).ToString());
    }
}
