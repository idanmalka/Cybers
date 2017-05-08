using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using QuickGraph;
using QuickGraph.Algorithms.KernighanLinAlgoritm;

namespace ILouvainLibrary
{
    public class ILouvain
    {
        public Partition<User> CalculatePartitions(AdjacencyGraph<User,Edge<User>> graph)
        {
            throw new NotImplementedException();
        }
    }
}
