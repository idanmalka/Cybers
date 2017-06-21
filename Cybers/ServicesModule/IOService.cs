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
                foreach (var user in users)
                {
                    var friendsDictionary = new Dictionary<string, User>();

                    foreach (var userFriendsId in user.FriendsIds)
                        friendsDictionary[userFriendsId.ToString()] = users[userFriendsId];

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

        public void SaveConfigurationToJson(List<string> clusterList, List<string> distributionList)
        {
            var configJson = JsonConvert.SerializeObject(new AlgorithmAttributesEventArgs
            {
                ClustringAttributes = clusterList,
                DistributingAttributes = distributionList
            });

            SaveFileDialog dlg = new SaveFileDialog
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
            }

        }
    }
}
