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
        public Dictionary<string,int> UsersSuspicionLevel { get; set; }
        public Partition Partition { get; set; }
    }
}
