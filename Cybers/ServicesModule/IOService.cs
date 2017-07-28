using Cybers.Infrustructure.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Cybers.Infrustructure.models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ServicesModule
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IOService : IIOService
    {
        public void OpenFileDialog(EventHandler<ServiceResult<string>> callback)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                callback.Invoke(this, new ServiceResult<string>(openFileDialog.FileName));
            }
        }

        public IEnumerable<User> ReadUsersFromPath(string path)
        {
            try
            {
                var usersJson = File.ReadAllText(path);
                var users = JsonConvert.DeserializeObject<List<User>>(usersJson);

                var friendShipDict = new Dictionary<int,List<int>>();
                
                foreach (var user in users)
                {
                    var userIndex = user.Index;
                    if (!friendShipDict.ContainsKey(userIndex))
                        friendShipDict[userIndex] = new List<int>();

                    foreach (var friendIndex in user.FriendsIndexs.Distinct().Where(i => i != userIndex)) //syncronize friendsLists
                    {
                        friendShipDict[userIndex].Add(friendIndex);

                        if (friendShipDict.ContainsKey(friendIndex))
                            friendShipDict[friendIndex].Add(userIndex);

                        else
                        {
                            friendShipDict[friendIndex] = new List<int> { userIndex };
                            users[friendIndex].FriendsIndexs.Add(userIndex);
                        }
                        
                    }

                    var friendsDictionary = new Dictionary<string, User>(); //add friends to friendsList

                    foreach (var userFriendIndex in friendShipDict[userIndex])
                        if (userIndex != userFriendIndex)
                            friendsDictionary[userFriendIndex.ToString()] = users[userFriendIndex];

                    user.FriendsList = friendsDictionary.Values.ToList();
                }

                return users;
            }
            catch (Exception)
            {
                throw new IncorrectUsersFileException();
            }
        }

        public AlgorithmAttributesEventArgs ReadConfigurationFromFile(string path)
        {
            try
            {
                var attributesJson = File.ReadAllText(path);
                var obj = JsonConvert.DeserializeObject<AlgorithmAttributesEventArgs>(attributesJson);

                return obj;
            }
            catch (Exception)
            {
                throw new IncorrectConfigurationFileException();
            }
        }

        public bool SaveConfigurationToJson(List<string> clusterList, List<string> distributionList, int threshold)
        {
            var configJson = JsonConvert.SerializeObject(new AlgorithmAttributesEventArgs
            {
                ClustringAttributes = clusterList,
                DistributingAttributes = distributionList,
                Threshold = threshold
            }, Formatting.Indented);

            var dlg = new SaveFileDialog
            {
                FileName = "Cybers Configuration",
                DefaultExt = ".json",
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, configJson);
                return true;
            }
            return false;
        }

        public bool ExportResultsToPajekFile(Partition results)
        {
            try
            {
                var users = results.Clusters.SelectMany(cluster => cluster.Verticies).OrderBy(user => user.Index).ToList();
                var textNetwork = new StringBuilder();
                var textCluster = new StringBuilder();
                textNetwork.AppendLine($"*Vertices {users.Count}");
                textCluster.AppendLine($"*Vertices {users.Count}");
                users.ForEach(user => textNetwork.AppendLine($"{user.Index + 1}"));
                textNetwork.AppendLine("*Edges");
                users.ForEach(user => user.FriendsIndexs.ForEach(i => textNetwork.AppendLine($"{user.Index + 1} {i + 1}")));
                var dlg = new SaveFileDialog
                {
                    FileName = "Cybers_Detection"
                };

                // Show save file dialog box
                var result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    var network = dlg.FileName + ".net";
                    var cluster = dlg.FileName + ".clu";
                    File.WriteAllText(network, textNetwork.ToString());
                    File.WriteAllText(cluster, textCluster.ToString());
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ExportResultsToFile(AlgorithmResultsEventArgs results)
        {
            var configJson = JsonConvert.SerializeObject(results, Formatting.Indented);

            var dlg = new SaveFileDialog
            {
                FileName = "Cybers Detection Results",
                DefaultExt = ".json",
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                var filename = dlg.FileName;
                File.WriteAllText(filename, configJson);
                return true;
            }
            return false;
        }

        public AlgorithmResultsEventArgs ImportPrevouseResultsFile(string path)
        {
            try
            {
                var resultsPath = File.ReadAllText(path);
                var results = JsonConvert.DeserializeObject<AlgorithmResultsEventArgs>(resultsPath);

                return results;
            }
            catch (Exception)
            {
                throw new IncorrectResultsFileException();
            }
        }
    }
}
