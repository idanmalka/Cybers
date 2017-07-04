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
        public Dictionary<double, long> UsersPerValue = new Dictionary<double, long>();

        [JsonIgnore]
        public Dictionary<double, long>.KeyCollection Keys => UsersPerValue.Keys;

        public long this[double key]
        {
            set => UsersPerValue[key] = value;
            get => UsersPerValue[key];
        }
    }
}
