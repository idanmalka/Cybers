using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class AlgorithmRunDataUpdateEventArgs: EventArgs
    {
        public Dictionary<long,long> ClustersUsersCount { get; set; }
    }
}
