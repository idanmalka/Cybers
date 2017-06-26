using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;

namespace CybersDetectionAlgorithm
{
    public class CybersDetectionResults
    {
        public Dictionary<string, double> UsersSuspicionLevel { get; set; }
        public Partition Partition { get; set; }

        /*dictionary that holds the distribution data:
         * Key: <clusterId,Attribute>
         * Value: Dictionary<Attribute name, number of users that posses this value in the cluster>
         * */
        public Dictionary<KeyValuePair<string,string>,Dictionary<long,long>> AttributesRarityMeasurement { get; set; }

        public IEnumerable<string> DistributionAttributes { get; set; }
        public IEnumerable<string> ClusteringAttributes { get; set; }
    }
}
