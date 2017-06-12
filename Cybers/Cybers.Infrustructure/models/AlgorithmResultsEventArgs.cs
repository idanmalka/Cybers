using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class AlgorithmResultsEventArgs
    {
        public Dictionary<string, double> UsersSuspicionLevel { get; set; }
        public Partition Partition { get; set; }
    }
}
