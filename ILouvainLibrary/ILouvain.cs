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
        private readonly IEnumerable<string> _clusteringAttributes;
        private long _twoM;
        private long _twoN;
        private int N;
        private List<User> _originalUsers;
        private List<User> _vertices;
        private Dictionary<long, List<User>> _adjacentVertices;
        private Dictionary<long, double> _verticesInertia;
        private int[][] _adjacency;
        private double[][] _auclideanDistance;
        private double _qinertia;
        private Dictionary<long, long> _clustersUsersCount;
        private Partition _partition;
        private User _gUser;

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
            _twoM = 2 * _graph.NumberOfEdges;
            N = _graph.Vertices.Count();
            _twoN = 2 * N;
            _vertices = graph.Vertices.ToList();
            _originalUsers = graph.Vertices.ToList();
            InitCenterOfGravityUser();
            _adjacency = BuildAdjacencyMatrix(_vertices,_graph);
            _adjacentVertices = BuildAdjacentVerticesList(_vertices);
            _auclideanDistance = BuildAuclideanDistancesMatrix(_vertices);
            _qinertia = CalculateInertia(_gUser,_vertices);
            _verticesInertia = BuildInertiaMatrix(_vertices);
            _clustersUsersCount = new Dictionary<long, long>();

            CreateDiscretePartition();
            _partition = new Partition(_graph);

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
                    foreach (var vertex in _vertices)
                    {
                        var oldClusterId = vertex.ClusterId;
                        vertex.ClusterId = FindNeighborClusterMaximizingQQplusGain(vertex, ref qqPlusCurrent, ref dirty);
                        if (vertex.ClusterId == oldClusterId) continue;
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
                    _partition = new Partition(_graph);
                    var fusionMatrixAdjRes = FusionMatrixAdjacency(_adjacency, _partition);
                    _graph = fusionMatrixAdjRes.Item1;
                    _adjacency = fusionMatrixAdjRes.Item2;
                    _auclideanDistance = FusionMatrixInertia(_auclideanDistance, _graph);
                    _qinertia = CalculateInertia(_gUser,_vertices);
                    _verticesInertia = BuildInertiaMatrix(_vertices);
                    _clustersUsersCount = new Dictionary<long, long>();
                    CreateDiscretePartition();
                }
                else end = true;
            } while (!end);
            
            _partition = new Partition(_originalUsers);

            ILouvainExecutionResult = _partition;
        }

        private int FindNeighborClusterMaximizingQQplusGain(User vertex, ref double qqPlusAnertior, ref bool dirty)
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

        private void InitCenterOfGravityUser()
        {
            var attr = _clusteringAttributes;
            _gUser = new User();

            foreach (var key in attr)
            {
                var sum = _vertices.Sum(vertex => vertex.Attributes[key]);
                _gUser.Attributes[key] = sum / N;
            }
            _gUser.Index = N;
        }

        private double[][] BuildAuclideanDistancesMatrix(List<User> vertices)
        {
            var auclideanDistance = new double[N + 1][];
            for (var index = 0; index < N + 1; index++)
                auclideanDistance[index] = new double[N + 1];
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
            var newUsersList = new List<User>();
            N = partition.Clusters.Count;
            //create new vertex for each cluster to replace all the contained users in a single one 
            for (var i = 0; i < N; i++)
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
                newUsersList.Add(clusterUser);
            }

            //connect the new vertices in the new graph
            foreach (var cluster in partition.Clusters)
            {
                var v1 = newUsersList[(int)cluster.Id];
                var friendsList = new List<User>();
                foreach (var neighbourId in cluster.NeighboursIds)
                {
                    friendsList.Add(newUsersList[neighbourId]);
                    v1.FriendsList = friendsList;
                    newGraph.AddEdge(v1, newUsersList[neighbourId]);
                }
            }

            //create new adjacenct matrix according to the new graph, and update constant data global fields for optimization 
            _twoM = 2 * newGraph.NumberOfEdges;
            _twoN = 2 * N;
            _vertices = newUsersList;
            _gUser.Index = N;
            var newAdjacency = BuildAdjacencyMatrix(newUsersList, newGraph);
            return new Tuple<Graph<User>, int[][]>(newGraph, newAdjacency);

        }

        private int[][] BuildAdjacencyMatrix(List<User> vertices, Graph<User> graph)
        {
            var adjacency = new int[N][];
            for (var index = 0; index < N; index++)
                adjacency[index] = new int[N];
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
            var newAuclidean = new double[N + 1][];
            for (var i = 0; i < N + 1; i++)
                newAuclidean[i] = new double[N + 1];
            
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


        private Dictionary<long, double> BuildInertiaMatrix(List<User> vertices)
        {
            var verticesInertia = new Dictionary<long, double>();
            foreach (var vertex in vertices)
                verticesInertia[vertex.Index] = CalculateInertia(vertex,vertices);

            return verticesInertia;
        }

        private double CalculateInertia(User v, List<User> vertices)
            => vertices.Sum(vertex => _auclideanDistance[v.Index][vertex.Index]);

        private Dictionary<long, List<User>> BuildAdjacentVerticesList(List<User> vertices)
        {
            var adjecentVerticeDict = new Dictionary<long, List<User>>();
            foreach (var vertex in vertices)
                adjecentVerticeDict[vertex.Index] = _graph.AdjacentVertices(vertex).ToList();

            return adjecentVerticeDict;
        }

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
            double sum = 0;

            for (var i = 0; i < N; i++)
                for (var j = i + 1; j < N; j++)
                {
                    var v1 = _vertices[i];
                    var v2 = _vertices[j];
                    if (Kroncker(v1.ClusterId, v2.ClusterId))
                        sum += CalculateQinertiaInner(v1, v2);
                }

            return double.IsNaN(sum) ? 0 : sum;
        }

        private double CalculateQinertiaInner(User v1, User v2)
        {
            var inertiaV1 = _verticesInertia[v1.Index];
            var inertiaV2 = _verticesInertia[v2.Index];
            var inertia = _qinertia;                    //if the vertices in the graph are identicle the inertia is 0, which displays NaN and does not fuze vertices into the same cluster
            var euclideanDistance = _auclideanDistance[v1.Index][v2.Index];
            return (inertiaV1 * inertiaV2 / Math.Pow(_twoN * inertia, 2)) - (euclideanDistance / (_twoN * inertia));
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
                        double kv1 = _graph.AdjacentVertices(v1).Count;
                        double kv2 = _graph.AdjacentVertices(v2).Count;
                        sum += _adjacency[v1.Index][v2.Index] - kv1 * kv2 / _twoM;
                    }
                }
            }
            var res = sum / _twoM;
            return double.IsNaN(res) ? 0 : res;
        }

        private double CalculateQQplus() => CalculateQinertia() + CalculateQng();

        private void CreateDiscretePartition()
        {
            for (var i = 0; i < N; i++)
            {
                _vertices[i].ClusterId = i;
                _clustersUsersCount[i] = 1;
            }

        }

        #endregion

    }
}
