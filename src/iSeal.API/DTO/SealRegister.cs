using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.API.DTO
{
    public class SealRegister
    {
        [DataType(DataType.Password)]
        public string SyncKey { get; set; }
    }
}
