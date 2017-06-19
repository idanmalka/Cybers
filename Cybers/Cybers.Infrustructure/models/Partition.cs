using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace Cybers.Infrustructure.models
{
    public class Partition
    {
        public List<Cluster> Clusters { get; set; }

        public Partition(UndirectedGraph<User, Edge<User>> graph)
        {
            var clusters = new Dictionary<long, List<User>>();
            Clusters = new List<Cluster>();

            //parse vertices from graph to dictionary for easy access of <clusterId,list<User>>
            foreach (var vertex in graph.Vertices)                  
                if (clusters.ContainsKey(vertex.ClusterId))
                    clusters[vertex.ClusterId].Add(vertex);
                else clusters.Add(vertex.ClusterId, new List<User> { vertex });

            //create new clusters according to the dictionary 
            Clusters = clusters.Select(c => new Cluster {Id = c.Key, Verticies = c.Value}).ToList();

            //connect neighbor clusters
            foreach (var vertex in graph.Vertices)
            {
                var userNeighbours = graph.AdjacentVertices(vertex);
                var cluster = Clusters.FirstOrDefault(c => c.Id == vertex.ClusterId);
                var neighbourClusters = new List<long>();

                foreach (var neighbour in userNeighbours)
                    if (neighbour.ClusterId != vertex.ClusterId && !neighbourClusters.Contains(neighbour.ClusterId))
                        neighbourClusters.Add(neighbour.ClusterId);

                if (cluster != null)
                    cluster.Neighbores = neighbourClusters;
            }


        }
    }
}
