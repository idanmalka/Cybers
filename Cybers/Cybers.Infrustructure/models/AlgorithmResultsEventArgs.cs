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
        [JsonProperty(nameof(UsersSuspicionLevel))]
        public Dictionary<string, double> UsersSuspicionLevel { get; set; }

        [JsonProperty(nameof(Partition))]
        public Partition Partition { get; set; }

        [JsonProperty(nameof(DistributionAttributes))]
        public IEnumerable<string> DistributionAttributes { get; set; }

        [JsonProperty(nameof(ClusteringAttributes))]
        public IEnumerable<string> ClusteringAttributes { get; set; }

        /*dictionary that holds the distribution data:
         * Key: <clusterId,Attribute>
         * Value: Dictionary<Attribute name, number of users that posses this value in the cluster>
         * */
        [JsonProperty(nameof(AttributesRarityMeasurement))]
        public Dictionary<KeyValuePair<string, string>, Dictionary<long, long>> AttributesRarityMeasurement { get; set; }
    }
}
