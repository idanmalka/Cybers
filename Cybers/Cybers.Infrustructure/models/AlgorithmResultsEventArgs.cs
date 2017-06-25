using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cybers.Infrustructure.models
{
    public class AlgorithmResultsEventArgs
    {
        [JsonProperty("userSuspicionLevel")]
        public Dictionary<string, double> UsersSuspicionLevel { get; set; }

        [JsonProperty("partition")]
        public Partition Partition { get; set; }
    }
}
