using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cybers.Infrustructure.models
{
    public class Address
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        public string CountryCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("code")]
        public long Code { get; set; }
    }
}
