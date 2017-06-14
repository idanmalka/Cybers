using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cybers.Infrustructure.converters
{
    public class FriendsListToNumberJSONConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (reader.Value as IEnumerable<int>)?.Count();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEnumerable<int>);
        }
    }
}
