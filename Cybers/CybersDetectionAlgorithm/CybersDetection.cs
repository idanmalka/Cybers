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

        #region Private Fields

        private readonly IEnumerable<User> _users;
        private readonly IEnumerable<string> _clusteringAttributes;
        private readonly IEnumerable<string> _distributionAttributes;
        private readonly double _threshold;
        private Dictionary<RarityKeyObject, RarityValueObject> _attributesRarityMeasurement { get; set; }

        #endregion

        #region Public Properties

        public AlgorithmResultsEventArgs LatestRunResults { get; set; }

        #endregion

        #region Events

        public event EventHandler InitializationFinished;
        public event EventHandler ClusteringFinished;
        public event EventHandler DistributingFinished;
        public event EventHandler InitializationStarted;
        public event EventHandler ClusteringStarted;
        public event EventHandler DistributingStarted;

        #endregion

        #region Methods

        public CybersDetection(IEnumerable<User> users, IEnumerable<string> clusteringAttributes,
            IEnumerable<string> distributionAttributes, double threshold)
        {
            _users = users;
            _clusteringAttributes = clusteringAttributes;
            _distributionAttributes = distributionAttributes;
            _threshold = threshold;
            _attributesRarityMeasurement = new Dictionary<RarityKeyObject, RarityValueObject>();
        }

        public void Execute()
        {
            InitializationStarted?.Invoke(this,null);
            var graph = CreateClusteringGraph();
            InitializationFinished?.Invoke(this, null);

            ClusteringStarted?.Invoke(this,null);
            var ilouvain = new ILouvain(graph);
            ilouvain.Execute();
            ClusteringFinished?.Invoke(this, null);

            DistributingStarted?.Invoke(this,null);
            Dictionary<string, double> userSuspicionLevel = CreateUserSuspicionLevels(ilouvain.ILouvainExecutionResult);
            DistributingFinished?.Invoke(this, null);

            LatestRunResults = new AlgorithmResultsEventArgs()
            {
                Partition = ilouvain.ILouvainExecutionResult,
                UsersSuspicionLevel = userSuspicionLevel,
                AttributesRarityMeasurement = _attributesRarityMeasurement,
                ClusteringAttributes = _clusteringAttributes,
                DistributionAttributes = _distributionAttributes
            };
        }

        private UndirectedGraph<User, Edge<User>> CreateClusteringGraph()
        {
            var graph = new UndirectedGraph<User, Edge<User>>();
            var clusteringUsers = new List<User>();

            foreach (var user in _users) //create all users for the graph containing only clustering attributes
            {
                var clusteringUser = new User
                {
                    Id = user.Id,
                    ClusterId = user.ClusterId,
                    FriendsIds = user.FriendsIds,
                    Index = user.Index
                };

                foreach (var clusteringAttribute in _clusteringAttributes)
                    clusteringUser.Attributes[clusteringAttribute] = user.Attributes[clusteringAttribute];

                clusteringUsers.Add(clusteringUser);
            }

            foreach (var clusteringUser in clusteringUsers) //create friendslist for each user after all users exist
            {
                var friendsDictionary = new Dictionary<string, User>();

                foreach (var friendId in clusteringUser.FriendsIds)
                    friendsDictionary[friendId.ToString()] = clusteringUsers[friendId];

                clusteringUser.FriendsList = friendsDictionary.Values.ToList();

                graph.AddVertex(clusteringUser); //add clutsering user to graph
            }

            foreach (var graphVertex in graph.Vertices) //create edges between clustering users in the graph
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
            {
                var clusterUsers = (from user in _users
                    from clusterVerticy in cluster.Verticies
                    where user.Id == clusterVerticy.Id
                    select user).ToList();

                foreach (var distributionAttribute in _distributionAttributes)
                {
                    var suspectedUsersByAttribute = IdentifyByDistribution(cluster.Id, clusterUsers, distributionAttribute);
                    foreach (var user in suspectedUsersByAttribute)
                        userSuspicionLevel[user.Id]++;
                }
            }

            //conversion to percentage
            foreach (var key in userSuspicionLevel.Keys.ToList())
                userSuspicionLevel[key] = (userSuspicionLevel[key] / _distributionAttributes.Count()) * 100;


            return userSuspicionLevel;
        }

        private IEnumerable<User> IdentifyByDistribution(long clusterId, List<User> clusterUsers, string distributionAttribute)
        {
            var rarityMeasurementPerValue = new Dictionary<long, double>();
           
            //count how many users posses each value of the given attribute
            foreach (var user in clusterUsers)
            {
                var value = user.Attributes[distributionAttribute];
                if (rarityMeasurementPerValue.ContainsKey(value))
                    rarityMeasurementPerValue[value]++;
                else rarityMeasurementPerValue[value] = 1;
            }

            //saving how many users in the cluster posses each value of the attribute
            //as in: for each <clusterId,AttributeName> save the value of <AttributeValue,NumberOfUsers>
            if (clusterUsers.Any())
            {
                var kvpkey = new RarityKeyObject
                {
                    ClusterId = clusterId.ToString(),
                    AttributeName = distributionAttribute
                };

                var kvpValue = new RarityValueObject();
                foreach (var key in rarityMeasurementPerValue.Keys)
                    kvpValue[key] = (long) rarityMeasurementPerValue[key];

                _attributesRarityMeasurement[kvpkey] = kvpValue;
            }

            //conversion to percentages
            var valuesCount = rarityMeasurementPerValue.Count;
            foreach (var key in rarityMeasurementPerValue.Keys.ToList())
                rarityMeasurementPerValue[key] = (rarityMeasurementPerValue[key] / valuesCount) * 100;

            return (from user in clusterUsers
                    from key in rarityMeasurementPerValue.Keys
                    where user.Attributes[distributionAttribute] == key && rarityMeasurementPerValue[key] <= _threshold
                    select user).ToList();
        }


        #endregion

    }
}
