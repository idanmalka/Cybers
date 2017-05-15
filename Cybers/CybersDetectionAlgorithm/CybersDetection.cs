using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using ILouvainLibrary;
using QuickGraph;

namespace CybersDetectionAlgorithm
{
    public class CybersDetection
    {
        private readonly IEnumerable<User> _users;
        private readonly IEnumerable<string> _clusteringAttributes;
        private readonly IEnumerable<string> _distributionAttributes;
        private readonly double _threshold;

        public CybersDetectionResults LatestRunResults { get; set; }

        public CybersDetection(IEnumerable<User> users, IEnumerable<string> clusteringAttributes,
            IEnumerable<string> distributionAttributes, double threshold)
        {
            _users = users;
            _clusteringAttributes = clusteringAttributes;
            _distributionAttributes = distributionAttributes;
            _threshold = threshold;
        }

        public CybersDetectionResults Execute()
        {
            var graph = CreateClusteringGraph();

            var ilouvain = new ILouvain(graph);
            ilouvain.Execute();

            Dictionary<string, double> userSuspicionLevel = CreateUserSuspicionLevels(ilouvain.ILouvainExecutionResult);

            return new CybersDetectionResults
            {
                Partition = ilouvain.ILouvainExecutionResult,
                UsersSuspicionLevel = userSuspicionLevel
            };
        }

        private UndirectedGraph<User, Edge<User>> CreateClusteringGraph()
        {
            var graph = new UndirectedGraph<User, Edge<User>>();

            foreach (var user in _users)
            {
                var clusteringUser = new User
                {
                    Id = user.Id,
                    ClusterId = user.ClusterId,
                    FriendsList = user.FriendsList
                };

                foreach (var clusteringAttribute in _clusteringAttributes)
                    clusteringUser.Attributes[clusteringAttribute] = user.Attributes[clusteringAttribute];

                graph.AddVertex(clusteringUser);
            }

            foreach (var graphVertex in graph.Vertices)
            foreach (var friend in graphVertex.FriendsList)
                graph.AddEdge(new Edge<User>(graphVertex, friend));

            return graph;
        }

        private Dictionary<string, double> CreateUserSuspicionLevels(Partition partition)
        {
            var userSuspicionLevel = new Dictionary<string, double>();
            foreach (var user in _users)
                userSuspicionLevel[user.Id] = 0;

            foreach (var cluster in partition.Clusters)
                foreach (var distributionAttribute in _distributionAttributes)
                {
                    var suspectedUsersByAttribute = IdentifyByDistribution(cluster, distributionAttribute);
                    foreach (var user in suspectedUsersByAttribute)
                        userSuspicionLevel[user.Id]++;
                }

            //conversion to percentage
            foreach (var key in userSuspicionLevel.Keys)
                userSuspicionLevel[key] = (userSuspicionLevel[key] / _distributionAttributes.Count()) * 100;
            

            return userSuspicionLevel;
        }

        private IEnumerable<User> IdentifyByDistribution(Cluster cluster, string distributionAttribute)
        {
            var rarityMeasurementPerValue = new Dictionary<long,double>();
            var clusterUsers = _users.Where(user => cluster.Verticies.Contains(user));

            foreach (var user in clusterUsers)
            {
                var value = user.Attributes[distributionAttribute];
                rarityMeasurementPerValue[value]++;
            }

            //conversion to percentages
            var valuesCount = rarityMeasurementPerValue.Count;
            foreach (var key in rarityMeasurementPerValue.Keys)
                rarityMeasurementPerValue[key] = (rarityMeasurementPerValue[key] / valuesCount) * 100;

            return (from user in clusterUsers
                    from key in rarityMeasurementPerValue.Keys
                    where user.Attributes[distributionAttribute] == key && rarityMeasurementPerValue[key] <= _threshold
                    select user).ToList();
        }

       


    }
}
