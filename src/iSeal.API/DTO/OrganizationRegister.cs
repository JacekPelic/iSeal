using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.API.DTO
{
    public class OrganizationRegister
    {
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
