using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cybers.Infrustructure.models
{
    public class RarityValueObject
    {
        [JsonProperty(nameof(UsersPerValue))]
        public Dictionary<long, long> UsersPerValue = new Dictionary<long, long>();

        [JsonIgnore]
        public Dictionary<long, long>.KeyCollection Keys => UsersPerValue.Keys;

        public long this[long key]
        {
            set => UsersPerValue[key] = value;
            get => UsersPerValue[key];
        }
    }
}
