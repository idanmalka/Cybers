using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Cybers.Infrustructure.models
{
    public class AlgorithmAttributesEventArgs
    {
        public List<UserAttribute> ClustringAttributes { get; set; }
        public List<UserAttribute> DistributingAttributes { get; set; }
        public string GraphFilePath { get; set; }
        public int Threshold { get; set; }
    }
}
