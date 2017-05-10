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
        private int _numberOfFriends;
        private TimeSpan _creationDate;
        private int _postsNumber;
        private Address _location;
        public string Id { get; set; }
        public string Nickname { get; set; }
        private int _favCount;
        private int _followersCount;
        private int _groups;
        private bool _verified;

        public bool Verified
        {
            get { return _verified; }
            set
            {
                _verified = value; 
                AddAttribute(_verified ? 1 : 0);
            }
        }

        public int Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                AddAttribute(_groups);
            }
        }


        public int FavCount
        {
            get { return _favCount; }
            set
            {
                _favCount = value; 
                AddAttribute(_favCount);
            }
        }

        public int FollowersCount
        {
            get { return _followersCount; }
            set
            {
                _followersCount = value;
                AddAttribute(_followersCount);
            }
        }

        public int NumberOfFriends
        {
            get { return _numberOfFriends; }
            set
            {
                _numberOfFriends = value;
                AddAttribute(_numberOfFriends);
            }
        }

        public TimeSpan CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value; 
                var now = new TimeSpan(DateTime.Today.Ticks).TotalMilliseconds;
                AddAttribute((long) (now - _creationDate.TotalMilliseconds));
            }
        }

        public Address Location
        {
            get { return _location; }
            set
            {
                _location = value; 
                AddAttribute(_location.Code);
            }
        }

        public int PostsNumber
        {
            get { return _postsNumber; }
            set
            {
                _postsNumber = value; 
                AddAttribute(_postsNumber);
            }
        }

        public Url ProfileLink { get; set; }

        private void AddAttribute(long value = 0, [CallerMemberName] string propertyName = null)
        {
            if (propertyName != null) Attributes[propertyName] = value;
        }

        public long ClusterId { get; set; }

    }
}
