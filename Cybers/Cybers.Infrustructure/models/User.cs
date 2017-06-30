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
        private List<int> _friendsIndexs;
        public Dictionary<string, long> Attributes { get; } = new Dictionary<string, long>();
        public long ClusterId { get; set; }

        [JsonProperty(nameof(Id))]
        public string Id { get; set; }

        public int Age
        {
            set => AddAttribute(value / 10 * 10);
        }

        [JsonIgnore]
        public List<User> FriendsList { get; set; }

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
        public List<int> FriendsIndexs
        {
            get => _friendsIndexs;
            set
            {
                _friendsIndexs = value;
                NumberOfFriends = value.Count;
            }
        }

        public int NumberOfFriends
        {
            set => AddAttribute(value);
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
            set => AddAttribute(value);
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
