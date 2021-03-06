﻿using System;
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
        public IEnumerable<User> Verticies { get; set; }
        public long Id { get; set; }
        public IEnumerable<long> Neighbores { get; set; }
    }
}
