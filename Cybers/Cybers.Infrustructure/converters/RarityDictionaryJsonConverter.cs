using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using Newtonsoft.Json;

namespace Cybers.Infrustructure.converters
{
    class RarityDictionaryJsonConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Dictionary<RarityKeyObject, RarityValueObject>) value;

            var parsingDictionary = new Dictionary<string, Dictionary<double, long>>();

            foreach (var objKey in obj.Keys)
            {
                var kvpKey = objKey.ClusterId + " " + objKey.AttributeName;
                parsingDictionary[kvpKey] = obj[objKey].UsersPerValue;
            }

            writer.WriteValue(JsonConvert.SerializeObject(parsingDictionary));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<double, long>>>(reader.Value as string);
            var parsedDictionary = new Dictionary<RarityKeyObject, RarityValueObject>();

            foreach (var objKey in obj.Keys)
            {
                var splited = objKey.Split(' ');
                var rarityKeyObj = new RarityKeyObject();
                rarityKeyObj.ClusterId = splited[0];
                rarityKeyObj.AttributeName = splited[1];

                parsedDictionary[rarityKeyObj] = new RarityValueObject();
                parsedDictionary[rarityKeyObj].UsersPerValue = obj[objKey];
            }

            return parsedDictionary;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

    }
}
