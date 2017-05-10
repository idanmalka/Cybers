using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class Partition
    {
        public IEnumerable<Cluster> Clusters { get; set; }

    }
}
