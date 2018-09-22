using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.integrationTest.Models
{
    public class Token
    {
        public string accessToken { get; set; }
        public DateTime expiration { get; set; }
    }
}
