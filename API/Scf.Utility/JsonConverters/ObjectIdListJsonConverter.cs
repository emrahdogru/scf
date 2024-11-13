using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Utility.JsonConverters
{

    public class ObjectIdListJsonConverter : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(List<ObjectId>);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
            ObjectId.Parse(reader.Value as string);

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var list = (List<ObjectId>)(value ?? new List<ObjectId>() {  });
            string strValue = "[" + string.Join(", ", list.Select(x => $"\"{x}\"")) + "]";
            writer.WriteValue(strValue.ToString());
        }
    }
}
