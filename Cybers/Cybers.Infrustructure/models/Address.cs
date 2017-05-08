using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    public class Address
    {
        public string Id { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public long Code { get; set; }
    }
}
