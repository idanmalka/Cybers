using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using QuickGraph.Algorithms.KernighanLinAlgoritm;

namespace CybersDetectionAlgorithm
{
    public class CybersDetectionResults
    {
        public Dictionary<string,int> UsersSuspicionLevel { get; set; }
        public Partition<User> Partition { get; set; }
    }
}
