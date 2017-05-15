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
    }
}
