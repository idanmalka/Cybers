using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.converters;
using Newtonsoft.Json;

namespace Cybers.Infrustructure.models
{
    public class Cluster
    {
        public List<User> Verticies { get; set; }
        public long Id { get; set; }
        public List<int> NeighboursIds { get; set; }

        [JsonIgnore]
        public List<User> Neighbours { get; set; }
    }
}
