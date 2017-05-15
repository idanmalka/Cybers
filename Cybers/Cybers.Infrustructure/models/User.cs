using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class User
    {
        public Dictionary<string, long> Attributes { get; } = new Dictionary<string, long>();
        public string Id { get; set; }
        public string Nickname { get; set; }
        public List<User> FriendsList { get; set; }
        public long ClusterId { get; set; }
        public Url ProfileLink { get; set; }

        public bool Verified
        {
            set => AddAttribute(value ? 1 : 0);
        }

        public int Groups
        {
            set => AddAttribute(value);
        }


        public int FavCount
        {
            set => AddAttribute(value);
        }

        public int FollowersCount
        {
            set => AddAttribute(value);
        }

        public int NumberOfFriends
        {
            set => AddAttribute(value);
        }
        public Address Location
        {
            set => AddAttribute(value.Code);
        }

        public int PostsNumber
        {
            set => AddAttribute(value);
        }

        public TimeSpan CreationDate
        {
            set
            {
                var now = new TimeSpan(DateTime.Today.Ticks).TotalMilliseconds;
                AddAttribute((long)(now - value.TotalMilliseconds));
            }
        }


        private void AddAttribute(long value = 0, [CallerMemberName] string propertyName = null)
        {
            if (propertyName != null) Attributes[propertyName] = value;
        }
    }
}
