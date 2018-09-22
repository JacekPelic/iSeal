using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.API.DTO
{
    public class SealAccessResponse
    {
        public Guid Guid { get; set; }
        public string SyncKey { get; set; }
    }
}
