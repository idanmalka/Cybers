using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cybers.Infrustructure.models
{
    public class RarityKeyObject: IEquatable<RarityKeyObject>
    {
        [JsonProperty(nameof(ClusterId))]
        public string ClusterId { get; set; }

        [JsonProperty(nameof(AttributeName))]
        public string AttributeName { get; set; }

        public RarityKeyObject()
        {
            
        }

        public bool Equals(RarityKeyObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ClusterId, other.ClusterId) && string.Equals(AttributeName, other.AttributeName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RarityKeyObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ClusterId != null ? ClusterId.GetHashCode() : 0) * 397) ^ 
                    (AttributeName != null ? AttributeName.GetHashCode() : 0);
            }
        }
    }
}
