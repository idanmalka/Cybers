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
        private UndirectedGraph<User, Edge<User>> _graph;

        public ILouvain(UndirectedGraph<User, Edge<User>> graph)
        {
            _graph = graph;
        }

        private double CalculateEuclideanDistance(User u1, User u2)
        {
            double distance = 0;
            foreach (var key in u1.Attributes.Keys)
            {
                var val1 = u1.Attributes[key];
                var val2 = u2.Attributes[key];
                distance += Math.Pow(val1 - val2, 2);
            }
            return distance;
        }

        private User CalculateCenterOfGravity(IEnumerable<User> verticies)
        {
            var attr = verticies.First().Attributes.Keys;
            var gUser = new User();
            var size = verticies.Count();
           
            foreach (var key in attr)
            {
                var sum = verticies.Sum(vertex => vertex.Attributes[key]);
                gUser.Attributes[key] = sum/size;
            }

            return gUser;
        }

        private double CalculateInertia(IEnumerable<User> verticies)
        {
            var gUser = CalculateCenterOfGravity(verticies);
            return CalculateInertia(verticies, gUser);
        }

        private double CalculateInertia(IEnumerable<User> verticies, User v)
            => verticies.Sum(vertex => CalculateEuclideanDistance(vertex, v));

        private double CalculateQinertia()
        {
            var count = _graph.VertexCount;
            var verticies = _graph.Vertices.ToList();
            double sum = 0;
            for (var i = 0; i < count; i++)
            {
                for (var j = i+1; j < count; j++)
                {
                    var v1 = verticies[i];
                    var v2 = verticies[j];
                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                    {
                        sum += CalculateQinertiaInner(verticies, v1, v2);
                    }
                }
            }
            return sum;
        }

        private double CalculateQinertiaInner(IReadOnlyCollection<User> verticies, User v1, User v2)
        {
            var inertiaV1 = CalculateInertia(verticies, v1);
            var inertiaV2 = CalculateInertia(verticies, v2);
            var inertia = CalculateInertia(verticies);
            var twoN = verticies.Count*2;
            var euclideanDistance = CalculateEuclideanDistance(v1, v2);
            return inertiaV1*inertiaV2/Math.Pow(twoN*inertia, 2)-euclideanDistance/(twoN * inertia);
        }


        private static bool Kroncker(long c1, long c2) => c1 == c2;

        private double CalculateQng()
        {
            double sum = 0;
            var twoM = 2 * _graph.EdgeCount;
            var count = _graph.VertexCount;
            var verticies = _graph.Vertices.ToList();
            for (var i = 0; i < count; i++)
            {
                for (var j = i + 1; j < count; j++)
                {
                    var v1 = verticies[i];
                    var v2 = verticies[j];
                    
                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                    {
                        var adjacent = _graph.AdjacentVertices(v1).Contains(v2) ? 1 : 0;
                        var kv1 = _graph.AdjacentDegree(v1);
                        var kv2 = _graph.AdjacentDegree(v2);
  
                        sum += adjacent-kv1*kv2/twoM;
                    }
                }
            }
            return sum/twoM;
        }

        private double CalculateQQplus() => CalculateQinertia() + CalculateQng();

        private long FindNeighborClusterMaximizingQQplusGain(User vertex)
        {

            return 0;
        }

        private void CreateDiscretePartition()
        {
            var verticies = _graph.Vertices.ToList();
            var count = verticies.Count;
            for (var i = 0; i < count; i++)
            {
                verticies[i].ClusterId = i;
            }
        }

        public void Execute()
        {
            CreateDiscretePartition();
            var verticies = _graph.Vertices.ToList();
            var qqPlusAnterior = 0.0;
            do
            {
                qqPlusAnterior = CalculateQQplus();
                foreach (var vertex in verticies)
                {
                    vertex.ClusterId = FindNeighborClusterMaximizingQQplusGain(vertex);
                }
            } while (CalculateQQplus() > qqPlusAnterior);
        }
    }
}
