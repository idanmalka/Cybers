using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using QuickGraph.Algorithms.KernighanLinAlgoritm;

namespace CybersDetectionAlgorithm
{
    public class CybersDetection
    {
        private IEnumerable<User> _users;
        private IEnumerable<string> _clusteringAttributes;
        private IEnumerable<string> _distributionAttributes;

        public CybersDetectionResults LatestRunResults { get; set; }

        public CybersDetection(IEnumerable<User> users, IEnumerable<string> clusteringAttributes,
            IEnumerable<string> distributionAttributes)
        {
            _users = users;
            _clusteringAttributes = clusteringAttributes;
            _distributionAttributes = distributionAttributes;
        }

        public CybersDetectionResults Analyze()
        {
            throw new NotImplementedException();
        }
    }
}
