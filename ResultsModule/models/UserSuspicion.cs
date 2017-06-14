using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsModule.models
{
    public class UserSuspicion
    {
        public string Key { get; set; }
        public double Level { get; set; }

        public UserSuspicion()
        {
            
        }

        public UserSuspicion(string key, double level)
        {
            Key = key;
            Level = level;
        }

    }
}
