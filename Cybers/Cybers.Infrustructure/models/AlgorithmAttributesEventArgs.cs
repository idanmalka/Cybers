using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Events;

namespace Cybers.Infrustructure.models
{
    public class AlgorithmAttributesEventArgs
    {
        [JsonProperty("clusteringAttributes")]
        public List<string> ClustringAttributes { get; set; }

        [JsonProperty("distributingAttributes")]
        public List<string> DistributingAttributes { get; set; }

        public string GraphFilePath { get; set; }
        public int Threshold { get; set; }
    }
}
