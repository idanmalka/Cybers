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

        #region Private Fields

        private Graph<User> _graph;                               
        private int[][] _adjacency;
        private double[][] _auclideanDistance;
        private readonly IEnumerable<string> _clusteringAttributes;

        private User _gUser;
        private double _qinertia;
        private readonly List<User> _originalUsers;
        private Dictionary<long, double> _verticesInertia;
        private Dictionary<long, long> _clustersUsersCount;

        #endregion

        #region Properties

        public event EventHandler DataUpdateEvent;
        public Partition ILouvainExecutionResult { get; set; }

        #endregion

        #region Methods

        public ILouvain(Graph<User> graph, IEnumerable<string> clusteringAttributes)
        {
            _graph = graph;
            _clusteringAttributes = clusteringAttributes;
            _originalUsers = graph.Vertices.ToList();
            InitCenterOfGravityUser();
            _adjacency = BuildAdjacencyMatrix(graph);
            _auclideanDistance = BuildAuclideanDistancesMatrix(graph);
            _qinertia = CalculateInertia(_gUser, graph.Vertices);
            _verticesInertia = BuildInertiaMatrix(graph.Vertices);

            CreateDiscretePartition();
        }

        public void Execute()
        {
            DataUpdateEvent?.Invoke(this, new AlgorithmRunDataUpdateEventArgs
            {
                ClustersUsersCount = _clustersUsersCount
            });
            var qqPlusAnterior = 0.0;
            var end = false;
            do
            {
                var qqPlusCurrent = CalculateQQplus();
                bool dirty;
                do
                {
                    dirty = false;
                    foreach (var vertex in _graph.Vertices)
                    {
                        //finding best neighbour community for the vertex to be in
                        var oldClusterId = vertex.ClusterId;
                        vertex.ClusterId = FindNeighborClusterMaximizingQQplusGain(vertex, ref qqPlusCurrent, ref dirty);
                        if (vertex.ClusterId == oldClusterId) continue;

                        //updating and raising update event
                        _clustersUsersCount[oldClusterId]--;
                        _clustersUsersCount[vertex.ClusterId]++;
                        DataUpdateEvent?.Invoke(this, new AlgorithmRunDataUpdateEventArgs
                        {
                            ClustersUsersCount = _clustersUsersCount
                        });
                    }
                } while (dirty);

                if (qqPlusCurrent > qqPlusAnterior)
                {
                    qqPlusAnterior = qqPlusCurrent;
                    var partition = new Partition(_graph);

                    var fusionMatrixAdjRes = FusionMatrixAdjacency(_adjacency, partition);
                    _graph = fusionMatrixAdjRes.Item1;
                    _adjacency = fusionMatrixAdjRes.Item2;

                    _auclideanDistance = FusionMatrixInertia(_auclideanDistance, _graph);
                    _qinertia = CalculateInertia(_gUser,_graph.Vertices);
                    _verticesInertia = BuildInertiaMatrix(_graph.Vertices);

                    CreateDiscretePartition();
                }
                else end = true;
            } while (!end);
            
            ILouvainExecutionResult = new Partition(_originalUsers); 
        }

        private int FindNeighborClusterMaximizingQQplusGain(User vertex, ref double qqPlusAnertior, ref bool dirty)
        {
            var anteriorClusterId = vertex.ClusterId;
            var newClusterId = vertex.ClusterId;
            var checkedClusters = new HashSet<long>();

            foreach (var neighbour in _graph.AdjacentVertices(vertex))
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

        private void InitCenterOfGravityUser()
        {
            var n = _graph.NumberOfVertices;
            var attr = _clusteringAttributes;
            _gUser = new User();

            foreach (var key in attr)
            {
                var sum = _graph.Vertices.Sum(vertex => vertex.Attributes[key]);
                _gUser.Attributes[key] = sum / n;
            }
            _gUser.Index = n;
        }

        private double[][] BuildAuclideanDistancesMatrix(Graph<User> graph)
        {
            var n = graph.NumberOfVertices;
            var vertices = graph.Vertices.ToList();
            var auclideanDistance = new double[n + 1][];
            for (var index = 0; index < n + 1; index++)
                auclideanDistance[index] = new double[n + 1];
            foreach (var v1 in vertices)
            {
                var distanceToCenter = CalculateEuclideanDistance(v1, _gUser);
                auclideanDistance[v1.Index][_gUser.Index] = distanceToCenter;
                auclideanDistance[_gUser.Index][v1.Index] = distanceToCenter;

                foreach (var v2 in vertices)
                    if ((int)auclideanDistance[v1.Index][v2.Index] == 0)
                    {
                        var distance = CalculateEuclideanDistance(v1, v2);
                        auclideanDistance[v1.Index][v2.Index] = distance;
                        auclideanDistance[v2.Index][v1.Index] = distance;
                    }
            }

            return auclideanDistance;
        }

        private Tuple<Graph<User>, int[][]> FusionMatrixAdjacency(int[][] adjacency, Partition partition)
        {
            var newGraph = new Graph<User>();
            var n = partition.Clusters.Count;
            //create new vertex for each cluster to replace all the contained users in a single one 
            for (var i = 0; i < n; i++)
            {
                var cluster = partition.Clusters[i];
                var clusterUser = new User
                {
                    Index = i,
                    ContainedUsers = cluster.Verticies,
                    ClusterId = i,
                    FriendsIndexs = cluster.NeighboursIds
                };
                newGraph.AddVertex(clusterUser);
            }

            var newVertices = newGraph.Vertices.ToList();

            //connect the new vertices in the new graph
            foreach (var cluster in partition.Clusters)
            {
                var v1 = newVertices[(int)cluster.Id];
                var friendsList = new List<User>();
                foreach (var neighbourId in cluster.NeighboursIds)
                {
                    friendsList.Add(newVertices[neighbourId]);
                    v1.FriendsList = friendsList;
                    newGraph.AddEdge(v1, newVertices[neighbourId]);
                }
            }

            //create new adjacenct matrix according to the new graph, and update constant data global fields for optimization 
            _gUser.Index = n;
            var newAdjacency = BuildAdjacencyMatrix(newGraph);
            return new Tuple<Graph<User>, int[][]>(newGraph, newAdjacency);

        }

        private int[][] BuildAdjacencyMatrix(Graph<User> graph)
        {
            var n = graph.NumberOfVertices;
            var vertices = graph.Vertices.ToList();
            var adjacency = new int[n][];
            for (var index = 0; index < n; index++)
                adjacency[index] = new int[n];
            foreach (var v1 in vertices)
            foreach (var v2 in vertices)
                if (adjacency[v1.Index][v2.Index] == 0 && graph.IsAdjacentVertices(v1, v2))
                {
                    adjacency[v1.Index][v2.Index] = 1;
                    adjacency[v2.Index][v1.Index] = 1;
                }

            return adjacency;
        }

        private double[][] FusionMatrixInertia(double[][] auclideanDistance, Graph<User> graph)
        {
            var n = graph.NumberOfVertices;
            var newAuclidean = new double[n + 1][];
            for (var i = 0; i < n + 1; i++)
                newAuclidean[i] = new double[n + 1];
            
            //create new inertia matrix and update the values for all clusters and for the center of gravity
            foreach (var vertex in graph.Vertices)
            {
                var centerSum = CalculateNewAuclideanToCenter(vertex, auclideanDistance);
                newAuclidean[vertex.Index][_gUser.Index] = centerSum;
                newAuclidean[_gUser.Index][vertex.Index] = centerSum;
                foreach (var neighbour in _graph.AdjacentVertices(vertex))
                {
                    var sum = CalcNewAuclideanDistance(vertex, neighbour, auclideanDistance);
                    newAuclidean[vertex.Index][neighbour.Index] = sum;
                    newAuclidean[neighbour.Index][vertex.Index] = sum;
                }
            }

            return newAuclidean;
        }

        private double CalculateNewAuclideanToCenter(User vertex, double[][] auclideanDistance)
        {
            return vertex.ContainedUsers.Sum(formerUser => auclideanDistance[formerUser.Index][_gUser.Index]);
        }

        private double CalcNewAuclideanDistance(User vertex, User neighbour, double[][] auclideanDistance)
        {
            var sum = 0.0;

            foreach (var formerUser in vertex.ContainedUsers)
                foreach (var formerNeighbour in neighbour.ContainedUsers)
                {
                    sum += auclideanDistance[formerUser.Index][formerNeighbour.Index];
                }

            return sum;

        }


        private Dictionary<long, double> BuildInertiaMatrix(IEnumerable<User> vertices)
        {
            var verticesInertia = new Dictionary<long, double>();
            foreach (var vertex in vertices)
                verticesInertia[vertex.Index] = CalculateInertia(vertex,vertices);

            return verticesInertia;
        }

        private double CalculateInertia(User v, IEnumerable<User> vertices)
            => vertices.Sum(vertex => _auclideanDistance[v.Index][vertex.Index]);

        private double CalculateEuclideanDistance(User u1, User u2)
        {
            var distance = 0.0;
            foreach (var key in _clusteringAttributes)
            {
                var val1 = u1.Attributes[key];
                var val2 = u2.Attributes[key];
                distance += Math.Pow(val1 - val2, 2);
            }
            return distance;
        }

        private double CalculateQinertia()
        {
            var n = _graph.NumberOfVertices;
            var vertices = _graph.Vertices.ToList();
            double sum = 0;

            for (var i = 0; i < n; i++)
                for (var j = i + 1; j < n; j++)
                {
                    var v1 = vertices[i];
                    var v2 = vertices[j];
                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                        sum += CalculateQinertiaInner(v1, v2);
                }

            return double.IsNaN(sum) ? 0 : sum;
        }

        private double CalculateQinertiaInner(User v1, User v2)
        {
            double twoN = 2 * _graph.NumberOfVertices;
            var inertiaV1 = _verticesInertia[v1.Index];
            var inertiaV2 = _verticesInertia[v2.Index];
            var inertia = _qinertia;                    //if the vertices in the graph are identicle the inertia is 0, which displays NaN and does not fuze vertices into the same cluster
            var euclideanDistance = _auclideanDistance[v1.Index][v2.Index];
            return (inertiaV1 * inertiaV2 / Math.Pow(twoN * inertia, 2)) - (euclideanDistance / (twoN * inertia));
        }

        private static bool Kroncker(long c1, long c2) => c1 == c2;

        private double CalculateQng()
        {
            var n = _graph.NumberOfVertices;
            double sum = 0;
            double twoM = 2 * _graph.NumberOfEdges;
            var vertices = _graph.Vertices.ToList();
            for (var i = 0; i < n; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    var v1 = vertices[i];
                    var v2 = vertices[j];

                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                    {
                        double kv1 = _graph.AdjacentVertices(v1).Count;
                        double kv2 = _graph.AdjacentVertices(v2).Count;
                        sum += _adjacency[v1.Index][v2.Index] - kv1 * kv2 / twoM;
                    }
                }
            }
            var res = sum / twoM;
            return double.IsNaN(res) ? 0 : res;
        }

        private double CalculateQQplus() => CalculateQinertia() + CalculateQng();

        private void CreateDiscretePartition()
        {
            var n = _graph.NumberOfVertices;
            var vertices = _graph.Vertices.ToList();
            _clustersUsersCount = new Dictionary<long, long>();

            for (var i = 0; i < n; i++)
            {
                vertices[i].ClusterId = i;
                _clustersUsersCount[i] = 1;
            }

        }

        #endregion

    }
}
