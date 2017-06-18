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
    public class User
    {
        private List<int> _friendsIds;
        public Dictionary<string, long> Attributes { get; } = new Dictionary<string, long>();
        public long ClusterId { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        public List<User> FriendsList { get; set; }

        [JsonProperty("verified")]
        public bool Verified
        {
            set => AddAttribute(value ? 1 : 0);
        }

        [JsonProperty("groups")]
        public int Groups
        {
            set => AddAttribute(value);
        }


        [JsonProperty("friends")]
        public List<int> FriendsIds
        {
            get => _friendsIds;
            set
            {
                _friendsIds = value;
                NumberOfFriends = value.Count;
            }
        }

        public int NumberOfFriends
        {
            set => AddAttribute(value);
        }

        [JsonProperty("address")]
        public Address Address
        {
            set => AddAttribute(value.Code);
        }

        [JsonProperty("postsNumber")]
        public int PostsNumber
        {
            set => AddAttribute(value);
        }

        [JsonProperty("registered")]
        public DateTime CreationDate
        {
            set
            {
                var now = new TimeSpan(DateTime.Today.Ticks).TotalMilliseconds;
                AddAttribute((long)(now - value.Millisecond));
            }
        }


        private void AddAttribute(long value = 0, [CallerMemberName] string propertyName = null)
        {
            if (propertyName != null) Attributes[propertyName] = value;
        }
    }
}
