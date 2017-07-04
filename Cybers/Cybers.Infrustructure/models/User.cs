using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.converters;
using Newtonsoft.Json;

namespace Cybers.Infrustructure.models
{
    public class User : IEquatable<User>
    {
        private List<User> _friendsList;
        private int _clusterId;
        public Dictionary<string, double> Attributes { get; } = new Dictionary<string, double>();

        public int ClusterId
        {
            get => _clusterId;
            set
            {
                _clusterId = value;
                if (ContainedUsers.Count != 0)
                    foreach (var containedUser in ContainedUsers)
                    {
                        containedUser.ClusterId = value;
                    }
            }
        }

        [JsonIgnore]
        public List<User> ContainedUsers { get; set; } = new List<User>();

        [JsonProperty(nameof(Id))]
        public string Id { get; set; }

        public int Age
        {
            set => AddAttribute(value / 10 * 10);
        }

        [JsonIgnore]
        public List<User> FriendsList
        {
            get => _friendsList;
            set
            {
                _friendsList = value;
                NumberOfFriends = value.Count;
            }
        }

        [JsonProperty(nameof(Verified))]
        public bool Verified
        {
            set => AddAttribute(value ? 1 : 0);
        }

        [JsonProperty(nameof(Groups))]
        public int Groups
        {
            set => AddAttribute(value);
        }


        //[JsonProperty(nameof(FriendsIndexs))]
        [JsonProperty("FriendsIds")]
        public List<int> FriendsIndexs { get; set; }

        [JsonIgnore]
        public int NumberOfFriends
        {
            set => AddAttribute(value);
        }

        public string Gender
        {
            set => AddAttribute(value.Equals("male") ? 1 : 0);
        }

        //[JsonProperty(nameof(Address))]
        [JsonIgnore]
        public Address Address
        {
            set => AddAttribute(value.Code);
        }

        [JsonProperty(nameof(PostsNumber))]
        public int PostsNumber
        {
            set => AddAttribute(value / 100 * 100);
        }

        [JsonProperty(nameof(CreationDate))]
        public DateTime CreationDate
        {
            set => AddAttribute(value.Year);
        }

        private void AddAttribute(long value = 0, [CallerMemberName] string propertyName = null)
        {
            if (propertyName != null) Attributes[propertyName] = value;
        }

        public User()
        {
            FriendsList = new List<User>();
            //
        }

        [JsonProperty("Index")]
        public int Index { get; set; }

        public bool Equals(User other)
        {
            return other != null && Index == other.Index;
        }
    }
}
