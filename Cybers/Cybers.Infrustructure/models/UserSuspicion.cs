using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class UserSuspicion
    {
        public string Key { get; set; }
        public double Level { get; set; }
        public long ClusterId { get; set; }

        public UserSuspicion()
        {

        }

        public UserSuspicion(string key, double level, long cId)
        {
            Key = key;
            Level = level;
            ClusterId = cId;
        }

    }
}
