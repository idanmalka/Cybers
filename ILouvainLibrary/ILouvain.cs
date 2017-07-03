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
        private readonly IEnumerable<string> _clusteringAttributes;
        private readonly long _twoM;
        private readonly long _twoN;
        private readonly long N;
        private readonly List<User> _vertices;
        private readonly Dictionary<long, List<User>> _adjacentVertices;
        private readonly Dictionary<long, double> _verticesInertia;
        private readonly int[][] _adjacency;
        private readonly double[][] _auclideanDistance;
        private double _qinertia;

        private Dictionary<long, long> _clustersUsersCount;

        public event EventHandler DataUpdateEvent;
        public Partition ILouvainExecutionResult { get; set; }


        public ILouvain(Graph<User> graph, IEnumerable<string> clusteringAttributes)
        {
            _graph = graph;
            _clusteringAttributes = clusteringAttributes;
            _twoM = 2 * _graph.NumberOfEdges;
            N = _graph.Vertices.Count();
            _twoN = 2 * N;
            _vertices = graph.Vertices.ToList();
            _qinertia = CalculateInertia();
            _clustersUsersCount = new Dictionary<long, long>();

            _adjacency = BuildAdjacencyMatrix(_vertices);
            _adjacentVertices = BuildAdjacentVerticesList(_vertices);
            _auclideanDistance = BuildAuclideanDistancesMatrix(_vertices);
            _verticesInertia = BuildInertiaMatrix(_vertices);

        }

        public void Execute()
        {
            CreateDiscretePartition();
            DataUpdateEvent?.Invoke(this, new AlgorithmRunDataUpdateEventArgs
            {
                ClustersUsersCount = _clustersUsersCount
            });
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
                        var oldClusterId = vertex.ClusterId;
                        vertex.ClusterId = FindNeighborClusterMaximizingQQplusGain(vertex, ref qqPlusAnterior, ref dirty);
                        if (vertex.ClusterId == oldClusterId) continue;
                        _clustersUsersCount[oldClusterId]--;
                        _clustersUsersCount[vertex.ClusterId]++;
                        DataUpdateEvent?.Invoke(this, new AlgorithmRunDataUpdateEventArgs
                        {
                            ClustersUsersCount = _clustersUsersCount
                        });
                    }
                } while (dirty);
            } while (CalculateQQplus() > qqPlusAnterior);

            ILouvainExecutionResult = new Partition(_graph);
        }

        private Dictionary<long, double> BuildInertiaMatrix(List<User> vertices)
        {
            var verticesInertia = new Dictionary<long, double>();
            foreach (var vertex in vertices)
                verticesInertia[vertex.Index] = CalculateInertia(vertex);

            return verticesInertia;
        }

        private double[][] BuildAuclideanDistancesMatrix(List<User> vertices)
        {
            var auclideanDistance = new double[N][];
            for (var index = 0; index < N; index++)
                auclideanDistance[index] = new double[N];
            foreach (var v1 in vertices)
                foreach (var v2 in vertices)
                    if ((int)auclideanDistance[v1.Index][v2.Index] == 0)
                    {
                        var distance = CalculateEuclideanDistance(v1, v2);
                        auclideanDistance[v1.Index][v2.Index] = distance;
                        auclideanDistance[v2.Index][v2.Index] = distance;
                    }

            return auclideanDistance;
        }

        private Dictionary<long, List<User>> BuildAdjacentVerticesList(List<User> vertices)
        {
            var adjecentVerticeDict = new Dictionary<long, List<User>>();
            foreach (var vertex in vertices)
                adjecentVerticeDict[vertex.Index] = _graph.AdjacentVertices(vertex).ToList();

            return adjecentVerticeDict;
        }

        private double CalculateEuclideanDistance(User u1, User u2)
        {
            double distance = 0;
            foreach (var key in _clusteringAttributes)
            {
                var val1 = u1.Attributes[key];
                var val2 = u2.Attributes[key];
                distance += Math.Pow(val1 - val2, 2);
            }
            return distance;
        }

        private int[][] BuildAdjacencyMatrix(List<User> vertices)
        {
            var adjacency = new int[N][];
            for (var index = 0; index < N; index++)
                adjacency[index] = new int[N];
            foreach (var v1 in vertices)
                foreach (var v2 in vertices)
                    if (adjacency[v1.Index][v2.Index] == 0 && v1.FriendsIndexs.Contains(v2.Index))
                    {
                        adjacency[v1.Index][v2.Index] = 1;
                        adjacency[v2.Index][v1.Index] = 1;
                    }

            return adjacency;
        }

        private User CalculateCenterOfGravity()
        {
            var gUser = new User();

            foreach (var key in _clusteringAttributes)
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
                        sum += _adjacency[v1.Index][v2.Index] - kv1 * kv2 / _twoM;
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
            {
                _vertices[i].ClusterId = i;
                _clustersUsersCount[i] = 1;
            }

        }
    }
}
