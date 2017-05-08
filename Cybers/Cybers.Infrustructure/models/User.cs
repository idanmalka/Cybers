using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class User
    {
        public string Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int NumberOfFriends { get; set; }
        public string CreationDate { get; set; }
        public Address Location { get; set; }
        public int PostsNumber { get; set; }
        public Url ProfileLink { get; set; }

    }
}
