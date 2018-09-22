using System;
using System.Collections.Generic;
using System.Text;

namespace iSeal.Dal.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
