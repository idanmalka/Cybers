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

        public Partition()
        {
            
        }

        public Partition(Graph<User> graph)
        {
            var clustersTemp = new Dictionary<long, List<User>>();
            Clusters = new List<Cluster>();

            //parse vertices from graph to dictionary for easy access of <clusterId,list<User>>
            foreach (var vertex in graph.Vertices)                  
                if (clustersTemp.ContainsKey(vertex.ClusterId))
                    clustersTemp[vertex.ClusterId].Add(vertex);
                else clustersTemp.Add(vertex.ClusterId, new List<User> { vertex });

            //create new clusters according to the dictionary 
            //Clusters = clustersTemp.Select(c => new Cluster {Id = c.Key, Verticies = c.Value}).ToList();
            for (int i = 0; i < clustersTemp.Count; i++)
            {
                var cluster = new Cluster
                {
                    Id = i,
                    Verticies = clustersTemp.ElementAt(i).Value
                };
                foreach (var clusterVerticy in cluster.Verticies)
                {
                    clusterVerticy.ClusterId = i;
                }
                Clusters.Add(cluster);
            }

            //connect neighbor clusters
            foreach (var vertex in graph.Vertices)
            {
                var userNeighbours = graph.AdjacentVertices(vertex);
                var cluster = Clusters.FirstOrDefault(c => c.Id == vertex.ClusterId);
                var neighbourClusters = new List<int>();

                foreach (var neighbour in userNeighbours)
                    if (neighbour.ClusterId != vertex.ClusterId && !neighbourClusters.Contains(neighbour.ClusterId))
                        neighbourClusters.Add(neighbour.ClusterId);

                if (cluster != null)
                    cluster.NeighboursIds = neighbourClusters;
            }


        }

        public Partition(List<User> vertices)
        {
            var clustersTemp = new Dictionary<long, List<User>>();
            Clusters = new List<Cluster>();

            //parse vertices from graph to dictionary for easy access of <clusterId,list<User>>
            foreach (var vertex in vertices)
                if (clustersTemp.ContainsKey(vertex.ClusterId))
                    clustersTemp[vertex.ClusterId].Add(vertex);
                else clustersTemp.Add(vertex.ClusterId, new List<User> { vertex });

            //create new clusters according to the dictionary 
            //Clusters = clustersTemp.Select(c => new Cluster {Id = c.Key, Verticies = c.Value}).ToList();
            for (int i = 0; i < clustersTemp.Count; i++)
            {
                var cluster = new Cluster
                {
                    Id = i,
                    Verticies = clustersTemp.ElementAt(i).Value
                };
                foreach (var clusterVerticy in cluster.Verticies)
                {
                    clusterVerticy.ClusterId = i;
                }
                Clusters.Add(cluster);
            }

            //connect neighbor clusters
            foreach (var vertex in vertices)
            {
                var userNeighbours = vertex.FriendsList;
                var cluster = Clusters.FirstOrDefault(c => c.Id == vertex.ClusterId);
                var neighbourClusters = new List<int>();

                foreach (var neighbour in userNeighbours)
                    if (neighbour.ClusterId != vertex.ClusterId && !neighbourClusters.Contains(neighbour.ClusterId))
                        neighbourClusters.Add(neighbour.ClusterId);

                if (cluster != null)
                    cluster.NeighboursIds = neighbourClusters;
            }

        }
    }
}
