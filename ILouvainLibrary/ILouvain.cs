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
        private readonly Graph<User> _graph;
        private readonly long _twoM;
        private readonly long _twoN;
        private readonly long N;
        private readonly List<User> _vertices;
        private readonly Dictionary<long, List<User>> _adjacentVertices;
        private readonly Dictionary<long, double> _verticesInertia;
//        private readonly int[][] _adjacency;
        private readonly double[][] _auclideanDistance;
        private double _qinertia;

        public Partition ILouvainExecutionResult { get; set; }


        public ILouvain(Graph<User> graph)
        {
            _graph = graph;
            _twoM = 2 * _graph.NumberOfEdges;
            N = _graph.Vertices.Count();
            _twoN = 2 * N;
            _vertices = graph.Vertices.ToList();
            _qinertia = CalculateInertia();

//            _adjacency = new int[N][];
//            for (var index = 0; index < N; index++)
//                _adjacency[index] = new int[N];
//            foreach (var v1 in _vertices)
//                foreach (var v2 in _vertices)
//                    if (_adjacency[v1.Index][v2.Index] == 0 && v1.FriendsIndexs.Contains(v2.Index))
//                    {
//                        _adjacency[v1.Index][v2.Index] = 1;
//                        _adjacency[v2.Index][v1.Index] = 1;
//                    }


            _adjacentVertices = new Dictionary<long, List<User>>();
            foreach (var vertex in _vertices)
                _adjacentVertices[vertex.Index] = _graph.AdjacentVertices(vertex).ToList();

            _auclideanDistance = new double[N][];
            for (var index = 0; index < N; index++)
                _auclideanDistance[index] = new double[N];
            foreach (var v1 in _vertices)
                foreach (var v2 in _vertices)
                    if ((int)_auclideanDistance[v1.Index][v2.Index] == 0)
                    {
                        var distance = CalculateEuclideanDistance(v1, v2);
                        _auclideanDistance[v1.Index][v2.Index] = distance;
                        _auclideanDistance[v2.Index][v2.Index] = distance;
                    }

            _verticesInertia = new Dictionary<long, double>();
            foreach (var vertex in _vertices)
                _verticesInertia[vertex.Index] = CalculateInertia(vertex);
            
        }

        public void Execute()
        {
            CreateDiscretePartition();
            var qqPlusAnterior = 0.0;
            do
            {
                qqPlusAnterior = CalculateQQplus();
                bool dirty;
                do
                {
                    dirty = false;
                    foreach (var vertex in _vertices)
                    {
                        vertex.ClusterId = FindNeighborClusterMaximizingQQplusGain(vertex, ref qqPlusAnterior, ref dirty);
                    }
                } while (dirty);
            } while (CalculateQQplus() > qqPlusAnterior);

            ILouvainExecutionResult = new Partition(_graph);
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

        private User CalculateCenterOfGravity()
        {
            var attr = _vertices.First().Attributes.Keys;
            var gUser = new User();

            foreach (var key in attr)
            {
                var sum = _vertices.Sum(vertex => vertex.Attributes[key]);
                gUser.Attributes[key] = sum / N;
            }

            return gUser;
        }

        private double CalculateInertia()
        {
            var gUser = CalculateCenterOfGravity();
            var sum = _vertices.Sum(vertex => CalculateEuclideanDistance(gUser, vertex));
            return sum;
        }

        private double CalculateInertia(User v)
            => _vertices.Sum(vertex => _auclideanDistance[v.Index][vertex.Index]);

        private double CalculateQinertia()
        {
            double sum = 0;

            for (var i = 0; i < N; i++)
                for (var j = i + 1; j < N; j++)
                {
                    var v1 = _vertices[i];
                    var v2 = _vertices[j];
                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                        sum += CalculateQinertiaInner(v1, v2);
                }

            return sum;
        }

        private double CalculateQinertiaInner(User v1, User v2)
        {
            var inertiaV1 = _verticesInertia[v1.Index];
            var inertiaV2 = _verticesInertia[v2.Index];
            var inertia = _qinertia;
            var euclideanDistance = _auclideanDistance[v1.Index][v2.Index];
            return inertiaV1 * inertiaV2 / Math.Pow(_twoN * inertia, 2) - euclideanDistance / (_twoN * inertia);
        }

        private static bool Kroncker(long c1, long c2) => c1 == c2;

        private double CalculateQng()
        {
            double sum = 0;
            for (var i = 0; i < N; i++)
            {
                for (var j = i + 1; j < N; j++)
                {
                    var v1 = _vertices[i];
                    var v2 = _vertices[j];

                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                    {
                        var kv1 = v1.Attributes[nameof(v1.NumberOfFriends)];
                        var kv2 = v2.Attributes[nameof(v2.NumberOfFriends)];
                        sum += (_graph.IsAdjacentVertices(v1, v2) ? 1 : 0) - kv1 * kv2 / _twoM;
                    }
                }
            }
            return sum / _twoM;
        }

        private double CalculateQQplus() => CalculateQinertia() + CalculateQng();

        private long FindNeighborClusterMaximizingQQplusGain(User vertex, ref double qqPlusAnertior, ref bool dirty)
        {
            var anteriorClusterId = vertex.ClusterId;
            var newClusterId = vertex.ClusterId;
            var checkedClusters = new HashSet<long>();

            foreach (var neighbour in _adjacentVertices[vertex.Index])
            {
                if (vertex.ClusterId == neighbour.ClusterId
                    || checkedClusters.Contains(neighbour.ClusterId)) continue;

                checkedClusters.Add(neighbour.ClusterId);
                vertex.ClusterId = neighbour.ClusterId;
                var qqPlusNew = CalculateQQplus();
                if (qqPlusAnertior < qqPlusNew)
                {
                    qqPlusAnertior = qqPlusNew;
                    newClusterId = neighbour.ClusterId;
                    dirty = true;
                }
                vertex.ClusterId = anteriorClusterId;
            }
            return newClusterId;
        }

        private void CreateDiscretePartition()
        {
            for (var i = 0; i < N; i++)
                _vertices[i].ClusterId = i;

        }
    }
}
